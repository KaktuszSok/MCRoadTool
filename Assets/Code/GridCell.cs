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

	public static Color ToColour(this CellState s)
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
				return showCentreline ? Color.blue : CellState.ROAD.ToColour();
			default:
				return Color.magenta;
		}
	}
}

public class GridCell : MonoBehaviour
{
	static Color highlightedTint = Color.yellow;
	static float highlightStrength = 0.5f;

	[SerializeField] SpriteRenderer sprite;
	public Vector2Int position { get; private set; }

	public CellState state;
	public CellState previewState;
	public bool highlighted { get; private set; }

	private void Awake()
	{
		previewState = CellState.NONE;
		SetState(CellState.EMPTY);
	}

	public void SetPosition(int x, int y)
	{
		position = new Vector2Int(x, y);
		transform.localPosition = (Vector2)position;
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
		Color baseCol = state.ToColour();
		if(previewState != CellState.NONE)
		{
			baseCol = previewState.ToColour();
			if(previewState != CellState.EMPTY)
			{
				baseCol *= 0.85f;
			}
		}

		sprite.color = highlighted ? Color.Lerp(baseCol, highlightedTint, highlightStrength) : baseCol;
	}
}