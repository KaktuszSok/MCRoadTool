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

	public CellState GetCellColour(Vector2Int pos) { return GetCellState(pos.x, pos.y); }
	public abstract CellState GetCellState(int x, int y);
	public virtual void OnKeyHeld() 
	{
		if(PlayerInput.showAdvancedInfo)
			MouseTooltip.extraFrameText = GetTooltipText();
	}
	public virtual string GetTooltipText()
	{
		return "Thickness: " + thickness + "\nRoad: " + roadCache + "\nFootpath: " + footpathCache;
	}

	public virtual CellState[,] GetGridChanges(CellGrid grid)
	{
		CellState[,] array = new CellState[grid.bounds.x, grid.bounds.y];

		for (int y = 0; y < grid.bounds.y; y++)
		{
			for (int x = 0; x < grid.bounds.x; x++)
			{
				CellState newState = GetCellState(x, y);
				if((int)newState > (int)grid.cells[x,y].state)
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
				grid.cells[x, y].SetPreviewState(newState);

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
				grid.cells[x, y].SetPreviewState(CellState.NONE);
			}
		}
	}

	public void ApplyChanges(CellGrid grid)
	{
		CellState[,] changes = GetGridChanges(grid);
		for (int y = 0; y < grid.bounds.y; y++)
		{
			for (int x = 0; x < grid.bounds.x; x++)
			{
				if(changes[x,y] != CellState.NONE)
					grid.cells[x, y].SetState(changes[x, y]);
			}
		}
	}
}