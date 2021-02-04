using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeArc : RoadShape
{
	/// <summary>
	/// A point on the circle's circumference which will determine its radius
	/// </summary>w
	protected Vector2Int endPoint;
	protected Vector2 tangent = Vector3.right*10;
	protected Vector2 centrePoint;
	protected float radius { get; private set; }
	protected float maxAngle;
	protected bool invertAngle = false;

	protected bool alternateMode = false;

	protected Transform tangentLineEffect;

	Stack<Vector2> tangentHistory = new Stack<Vector2>(HistoryUtils.historyLength);
	Stack<Vector2> tangentRedoHistory = new Stack<Vector2>(HistoryUtils.historyLength);

	public override string GetName()
	{
		return "Arc";
	}

	public override void OnKeyDown()
	{
		alternateMode = false;
		if(tangentLineEffect == null)
		{
			tangentLineEffect = GameObject.Instantiate(VisualsManager.instance.tangentLinePrefab).transform;
		}
		tangentLineEffect.gameObject.SetActive(true);
	}
	public override void OnKeyUp()
	{
		tangentLineEffect.gameObject.SetActive(false);
	}

	public override void OnKeyHeld()
	{
		if(Input.GetMouseButtonDown(1))
		{
			alternateMode = true;
		}
		if(Input.GetMouseButtonUp(1))
		{
			alternateMode = false;
		}

		if (alternateMode)
		{
			tangent = startPoint - PlayerInput.mousePos;
		}
		else
		{
			endPoint = PlayerInput.mousePos;
		}
		if (tangent == Vector2.zero)
			tangent = Vector2.right;
		tangentLineEffect.right = tangent;
		tangentLineEffect.position = new Vector3(startPoint.x, startPoint.y, tangentLineEffect.position.z) + (Vector3)Vector2.one*0.5f;
		Debug.DrawRay((Vector2)startPoint - tangent, tangent, Color.green);
		Debug.DrawLine((Vector2)startPoint, (Vector2)endPoint, Color.red);

		//calculate centrepoint:
		Vector2 perp = Vector2.Perpendicular(tangent);
			Debug.DrawRay((Vector2)startPoint, perp*10f, Color.green * 0.6f);
			Debug.DrawRay((Vector2)startPoint, -perp*10f, Color.green * 0.6f);
		Vector2 endpointsMid = ((Vector2)(startPoint + endPoint)) / 2f;
		Vector2 perp2 = Vector2.Perpendicular(endPoint - startPoint);
			Debug.DrawRay(endpointsMid, perp2*10f, Color.red * 0.6f);
			Debug.DrawRay(endpointsMid, -perp2*10f, Color.red * 0.6f);
		centrePoint = MathsUtils.FindLinesIntersect(startPoint, perp, endpointsMid, perp2);

		radius = (startPoint - centrePoint).magnitude;
		maxAngle = MathsUtils.DeltaAngle360(startPoint - centrePoint, endPoint - centrePoint);
		if (Vector2.SignedAngle(tangent, endPoint - startPoint) <= 0)
			invertAngle = true;
		else
			invertAngle = false;

		base.OnKeyHeld();
	}
	public override float CalculateCellDistance(int x, int y)
	{
		float angle = MathsUtils.DeltaAngle360(startPoint - centrePoint, new Vector2(x - centrePoint.x, y - centrePoint.y));
		if (angle > maxAngle != invertAngle && angle != 0) return Mathf.Infinity;

		float dx = x - centrePoint.x;
		float dy = y - centrePoint.y;
		return Mathf.Abs(Mathf.Sqrt(dx * dx + dy * dy) - radius); //distance from centreline of road
	}

	public override void OnAppliedChanges()
	{
		tangent = Vector2.Perpendicular(endPoint - centrePoint);
		if (invertAngle)
			tangent = -tangent;
		alternateMode = false;
	}

	public override Vector2Int GetNewStartPoint()
	{
		return endPoint;
	}

	public override void SaveToHistory(bool clearRedoHistory = true)
	{
		if (clearRedoHistory)
			tangentRedoHistory.Clear();

		if (tangentHistory.Count == HistoryUtils.historyLength) //history full - remove oldest
		{
			tangentHistory.RemoveOldest();
		}
		tangentHistory.Push(tangent);
	}
	public override void OnUndo()
	{
		if (tangentHistory.Count > 0)
		{
			tangentRedoHistory.Push(tangent);
			tangent = tangentHistory.Pop();
		}
	}
	public override void OnRedo()
	{
		if (tangentRedoHistory.Count > 0)
		{
			SaveToHistory(false);
			tangent = tangentRedoHistory.Pop();
		}
	}
}
