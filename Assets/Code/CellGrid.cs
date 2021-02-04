using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
	public static CellGrid instance;
	public GameObject cellPrefab;

	[Tooltip("Height of grid")]
	public int size = 64;
	public float aspectRatio = 16f / 9f;
	public Vector2Int bounds { get; private set; }

	public GridCell[,] cells = new GridCell[0,0];

	private void Awake()
	{
		instance = this;
	}

	void Start()
    {
		RegenerateGrid();
		RealignCam();
    }

	public void RegenerateGrid()
	{
		ClearGrid();
		bounds = new Vector2Int((int)(size * aspectRatio), size);
		cells = new GridCell[bounds.x, bounds.y];
		for(int y = 0; y < bounds.y; y++)
		{
			for(int x = 0; x < bounds.x; x++)
			{
				GridCell newCell = Instantiate(cellPrefab, transform).GetComponent<GridCell>();
				newCell.SetPosition(x, y);

				cells[x, y] = newCell;
			}
		}
	}

	public void ClearGrid()
	{
		for(int y = 0; y < cells.GetLength(1); y++)
		{
			for(int x = 0; x < cells.GetLength(0); x++)
			{
				Destroy(cells[x, y].gameObject);
			}
		}

		PlayerInput.currCell = null;
	}

	public void CleanseGrid()
	{
		SaveToHistory();
		for (int y = 0; y < cells.GetLength(1); y++)
		{
			for (int x = 0; x < cells.GetLength(0); x++)
			{
				cells[x, y].SetState(CellState.EMPTY);
			}
		}
	}

	public void RefreshGrid()
	{
		for (int y = 0; y < cells.GetLength(1); y++)
		{
			for (int x = 0; x < cells.GetLength(0); x++)
			{
				cells[x, y].UpdateColour();
			}
		}
	}

	public void SaveToHistory(bool clearRedoHistory = true)
	{
		for (int y = 0; y < cells.GetLength(1); y++)
		{
			for (int x = 0; x < cells.GetLength(0); x++)
			{
				cells[x, y].AddCurrentStateToHistory(clearRedoHistory);
			}
		}
		PlayerInput.SaveToHistory(clearRedoHistory);
	}
	public void Undo()
	{
		for (int y = 0; y < cells.GetLength(1); y++)
		{
			for (int x = 0; x < cells.GetLength(0); x++)
			{
				cells[x, y].RecoverPreviousState();
			}
		}
		PlayerInput.Undo();
	}
	public void Redo()
	{
		for (int y = 0; y < cells.GetLength(1); y++)
		{
			for (int x = 0; x < cells.GetLength(0); x++)
			{
				cells[x, y].RedoUndoneState();
			}
		}
		PlayerInput.Redo();
	}

	public void RealignCam()
	{
		Camera cam = Camera.main;

		Vector3 camPos = cam.transform.position;
		camPos.x = transform.position.x + (bounds.x-1f) / 2f;
		camPos.y = transform.position.y + (bounds.y-1f) / 2f;

		cam.orthographicSize = size / 2f;
		cam.transform.position = camPos;
	}

	public bool IsInBounds(Vector2Int pos)
	{
		if (pos.x < 0 || pos.y < 0 || pos.x >= bounds.x || pos.y >= bounds.y)
			return false;
		return true;
	}
}
