using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeEraser : ShapeLine
{
	public override CellState[,] GetGridChanges(CellGrid grid)
	{
		CellState[,] array = new CellState[grid.bounds.x, grid.bounds.y];

		for (int y = 0; y < grid.bounds.y; y++)
		{
			for (int x = 0; x < grid.bounds.x; x++)
			{
				array[x, y] = GetCellState(x, y);
			}
		}

		return array;
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
		Vector2 projection = startPoint + t * direction;
		float dist = Mathf.Sqrt((x - projection.x) * (x - projection.x) + (y - projection.y) * (y - projection.y));
		if (dist*2 < thickness)
		{
			return CellState.EMPTY;
		}

		return CellState.NONE;
	}
}
