using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeLine : RoadShape
{
	protected Vector2Int endPoint;
	protected Vector2 direction
	{
		get
		{
			return endPoint - startPoint;
		}
	}

	protected bool alternateMode = false;

	public override string GetName()
	{
		return "Line";
	}

	public override void OnKeyHeld()
	{
		endPoint = PlayerInput.mousePos;

		if (Input.GetMouseButton(1))
			alternateMode = true;
		else
			alternateMode = false;

		base.OnKeyHeld();
	}
	public override float CalculateCellDistance(int x, int y)
	{
		Vector2 startToCell = new Vector2(x - startPoint.x, y - startPoint.y);
		float t = Vector2.Dot(startToCell, direction) / direction.sqrMagnitude;
		if (alternateMode) //cut off past endpoints
		{
			if (t < 0f || t > 1f) return Mathf.Infinity;
		}
		else //round past endpoints
		{
			t = Mathf.Clamp(t, 0f, 1f);
		}
		Vector2 projection = startPoint + t*direction;
		float dx = (x - projection.x);
		float dy = (y - projection.y);
		return Mathf.Sqrt(dx*dx + dy*dy);
	}
}
