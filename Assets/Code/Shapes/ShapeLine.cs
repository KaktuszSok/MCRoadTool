using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeLine : RoadShape
{
	public Vector2Int endPoint;
	protected Vector2 direction
	{
		get
		{
			return endPoint - startPoint;
		}
	}

	protected bool alternateMode = false;

	public override void OnKeyHeld()
	{
		endPoint = PlayerInput.mousePos;

		if (Input.GetMouseButton(1))
			alternateMode = true;
		else
			alternateMode = false;

		base.OnKeyHeld();
	}
	public override CellState GetCellState(int x, int y)
	{
		Vector2 startToCell = new Vector2(x - startPoint.x, y - startPoint.y);
		float t = Vector2.Dot(startToCell, direction) / direction.sqrMagnitude;
		if (alternateMode)
		{
			if (t < 0f || t > 1f) return CellState.NONE;
		}
		else
		{
			t = Mathf.Clamp(t, 0f, 1f);
		}
		Vector2 projection = startPoint + t*direction;
		float dist = Mathf.Sqrt((x - projection.x) * (x - projection.x) + (y - projection.y) * (y - projection.y));
		if(dist*2 < thickness)
		{
			if (dist <= 0.5f) return CellState.CENTRELINE;
			else if (dist*2 < thickness - footpathWidth*2) return CellState.ROAD;
			else return CellState.FOOTPATH;
		}
			
		return CellState.NONE;
	}
}
