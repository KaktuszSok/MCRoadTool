using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoadShape
{
	public static float thickness = 12f;
	public static float footpathWidth = 2.0f;
	public Vector2Int startPoint;

	int roadCache = 0;
	int footpathCache = 0;

	public abstract string GetName();

	public float CalculateCellDistance(Vector2Int pos) { return CalculateCellDistance(pos.x, pos.y); }
	public abstract float CalculateCellDistance(int x, int y);
	public CellState GetCellState(Vector2Int pos) { return GetCellState(pos.x, pos.y); }
	public virtual CellState GetCellState(int x, int y)
	{
		float dist = CalculateCellDistance(x, y);
		if (dist * 2 < thickness)
		{
			if (dist <= 0.5f) return CellState.CENTRELINE;
			else if (dist * 2 < thickness - footpathWidth * 2) return CellState.ROAD;
			else return CellState.FOOTPATH;
		}

		return CellState.NONE;
	}
	public virtual void OnKeyDown() { }
	public virtual void OnKeyHeld() 
	{
		if(PlayerInput.showAdvancedInfo)
			MouseTooltip.extraFrameText = GetTooltipText();
	}
	public virtual string GetTooltipText()
	{
		return "Thickness: " + thickness + "\nRoad: " + roadCache + "\nFootpath: " + footpathCache;
	}
	public virtual void OnKeyUp() { }

	public virtual CellState[,] GetGridChanges(CellGrid grid)
	{
		CellState[,] array = new CellState[grid.bounds.x, grid.bounds.y];

		for (int y = 0; y < grid.bounds.y; y++)
		{
			for (int x = 0; x < grid.bounds.x; x++)
			{
				CellState newState = GetCellState(x, y);
				if((int)newState > (int)grid.GetCellAt(x,y).state)
					array[x, y] = newState;
			}
		}

		return array;
	}
	
	public void PreviewChanges(CellGrid grid)
	{
		CellState[,] changes = GetGridChanges(grid);
		if (PlayerInput.showAdvancedInfo)
			roadCache = footpathCache = 0;
		for (int y = 0; y < grid.bounds.y; y++)
		{
			for(int x = 0; x < grid.bounds.x; x++)
			{
				CellState newState = changes[x, y];
				grid.GetCellAt(x, y).SetPreviewState(newState);

				if (PlayerInput.showAdvancedInfo)
				{
					if (newState == CellState.ROAD || newState == CellState.CENTRELINE)
						roadCache++;
					if (newState == CellState.FOOTPATH)
						footpathCache++;
				}
			}
		}
	}
	public static void ClearPreview(CellGrid grid)
	{
		for (int y = 0; y < grid.bounds.y; y++)
		{
			for (int x = 0; x < grid.bounds.x; x++)
			{
				grid.GetCellAt(x, y).SetPreviewState(CellState.NONE);
			}
		}
	}

	public void ApplyChanges(CellGrid grid)
	{
		grid.SaveToHistory();
		CellState[,] changes = GetGridChanges(grid);
		for (int y = 0; y < grid.bounds.y; y++)
		{
			for (int x = 0; x < grid.bounds.x; x++)
			{
				if(changes[x,y] != CellState.NONE)
					grid.GetCellAt(x, y).SetState(changes[x, y]);
			}
		}

		OnAppliedChanges();
	}
	public virtual void OnAppliedChanges() { }

	/// <returns>Position of new start point or (int.MinValue, int.MinValue) if it doesn't implement such custom behaviour </returns>
	public virtual Vector2Int GetNewStartPoint() { return new Vector2Int(int.MinValue, int.MinValue); }

	//for saving shape-specific runtime-persistent information (such as ShapeArc's tangent)
	public virtual void SaveToHistory(bool clearRedoHistory = true) { }
	public virtual void OnRedo() { }
	public virtual void OnUndo() { }
}