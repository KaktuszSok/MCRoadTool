using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public static PlayerInput instance;
	static Camera cam;
	static CellGrid grid;
	public PauseMenu menu;

	public static bool showAdvancedInfo = false;

	public static GridCell currCell;
	RoadShape roadShape;

	Dictionary<KeyCode, RoadShape> shapesDict = new Dictionary<KeyCode, RoadShape>();

	public static Vector2Int mousePos
	{
		get
		{
			Vector3 floatPos = cam.ScreenToWorldPoint(Input.mousePosition);
			return new Vector2Int(Mathf.RoundToInt(Mathf.Clamp(floatPos.x, 0, grid.bounds.x-1)), Mathf.RoundToInt(Mathf.Clamp(floatPos.y, 0, grid.bounds.y-1)));
		}
	}

	void GenerateShapesDict()
	{
		shapesDict.Clear();
		shapesDict.Add(KeyCode.L, new ShapeLine());
		shapesDict.Add(KeyCode.E, new ShapeEraser());
	}

	void Awake()
    {
		instance = this;
		cam = Camera.main;
		grid = GetComponent<CellGrid>();
		
		GenerateShapesDict();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			RoadShape.ClearPreview(grid);
			roadShape = null;

			menu.ToggleOpen(!menu.isOpen);
		}

		if(!menu.isOpen)
		{
			HandleNormalInput();
		}
		else
		{
			HandleMenuInput();
		}
	}

	void HandleNormalInput()
    {
		if(Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))
		{
			grid.CleanseGrid();
		}
		if(Input.GetKeyDown(KeyCode.Period))
		{
			CellStateExtensions.showCentreline = !CellStateExtensions.showCentreline;
			grid.RefreshGrid();
		}
		if (Input.GetKeyDown(KeyCode.Comma))
		{
			showAdvancedInfo = !showAdvancedInfo;
			grid.RefreshGrid();
		}

		if (currCell != null)
		{
			foreach (KeyCode key in shapesDict.Keys)
			{
				if (Input.GetKeyDown(key))
				{
					roadShape = shapesDict[key];
					roadShape.startPoint = currCell.position;
				}
				if (Input.GetKey(key) && roadShape == shapesDict[key])
				{
					roadShape.OnKeyHeld();
					roadShape.PreviewChanges(grid);

					if(Input.mouseScrollDelta.y > 0.1f || Input.mouseScrollDelta.y < -0.1f)
					{
						RoadShape.thickness = Mathf.Clamp(RoadShape.thickness + (1.0f * Mathf.Sign(Input.mouseScrollDelta.y)), 1.0f, grid.size);
					}
					if(Input.GetMouseButtonDown(0))
					{
						roadShape.ApplyChanges(grid);
					}
				}
				if (Input.GetKeyUp(key) && roadShape == shapesDict[key])
				{
					RoadShape.ClearPreview(grid);
					roadShape = null;
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			HighlightCell(mousePos);
			if(roadShape != null && currCell != null)
				roadShape.startPoint = currCell.position;
		}
	}

	void HandleMenuInput()
	{

	}

	public void HighlightCell(Vector2Int pos)
	{
		if (!grid.IsInBounds(pos)) return;
	
		//Deselect current cell
		if (currCell) currCell.Highlight(false);
		if (!currCell || currCell.position != pos) //we selected a different cell
		{
			currCell = grid.cells[pos.x, pos.y];
			currCell.Highlight(true);
		}
		else //we clicked the already selected cell, so we just wanted to deselect it
		{
			currCell = null;
		}
	}
}
