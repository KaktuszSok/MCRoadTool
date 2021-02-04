using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
	NONE, //Should only be used for preview!!
	EMPTY,
	FOOTPATH,
	ROAD,
	CENTRELINE
}
public static class CellStateExtensions
{
	public static bool showCentreline = true;

	public static Color32 ToColour(this CellState s)
	{
		switch (s)
		{
			case CellState.EMPTY:
				return Color.white;
			case CellState.FOOTPATH:
				return Color.grey;
			case CellState.ROAD:
				return Color.black;
			case CellState.CENTRELINE:
				return showCentreline ? new Color32(0, 0, 255, 255) : CellState.ROAD.ToColour();
			default:
				return Color.magenta;
		}
	}
}

public class GridCell
{
	static Color32 highlightedTint = new Color32(255, 255, 0, 255);
	static float highlightStrength = 0.5f;

	public Vector2Int position { get; private set; }
	public CellChunk chunk { get; private set; }

	public CellState state;
	public CellState previewState;
	public bool highlighted { get; private set; }
	public Color32 colour = new Color32(0, 0, 0, 0);

	public Stack<CellState> stateHistory = new Stack<CellState>(HistoryUtils.historyLength);
	public Stack<CellState> redoHistory = new Stack<CellState>(HistoryUtils.historyLength);

	public void Awake(CellChunk ownerChunk)
	{
		chunk = ownerChunk;
		previewState = CellState.NONE;
		SetState(CellState.EMPTY);
	}

	public void SetPosition(int x, int y)
	{
		position = new Vector2Int(x, y);
	}

	public void SetState(CellState newState)
	{
		state = newState;
		UpdateColour();
	}

	public void SetPreviewState(CellState preview)
	{
		previewState = preview;
		UpdateColour();
	}

	public void Highlight(bool highlight)
	{
		highlighted = highlight;
		UpdateColour();
	}

	public void UpdateColour()
	{
		Color32 prevColour = colour;
		Color32 baseCol = state.ToColour();
		if(previewState != CellState.NONE)
		{
			baseCol = previewState.ToColour();
			if(previewState != CellState.EMPTY)
			{
				baseCol = new Color32((byte)(baseCol.r*0.85f), (byte)(baseCol.g * 0.85f), (byte)(baseCol.b * 0.85f), (byte)(baseCol.a * 0.85f));
			}
		}

		colour = highlighted ? Color32.Lerp(baseCol, highlightedTint, highlightStrength) : baseCol;
		if(!chunk.isDirty && colour.r != prevColour.r || colour.g != prevColour.g || colour.b != prevColour.b || colour.a != prevColour.a)
		{
			chunk.isDirty = true;
		}
	}

	public void AddCurrentStateToHistory(bool clearRedoHistory = true)
	{
		if(clearRedoHistory)
			redoHistory.Clear();
		
		if(stateHistory.Count == HistoryUtils.historyLength) //history full - remove oldest
		{
			stateHistory.RemoveOldest();
		}
		stateHistory.Push(state);
	}
	public void RecoverPreviousState()
	{
		if (stateHistory.Count > 0)
		{
			redoHistory.Push(state);
			SetState(stateHistory.Pop());
		}
	}
	public void RedoUndoneState()
	{
		if (redoHistory.Count > 0)
		{
			AddCurrentStateToHistory(false);
			SetState(redoHistory.Pop());
		}
	}
}