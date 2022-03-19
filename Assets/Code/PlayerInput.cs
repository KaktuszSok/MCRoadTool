using System.Collections.Generic;
using Code.Menu;
using Code.Shapes;
using Code.Util;
using UnityEngine;

namespace Code
{
	public class PlayerInput : MonoBehaviour
	{
		public static PlayerInput instance;
		static Camera cam;
		static CellGrid grid;
		public PauseMenu menu;

		public static bool showAdvancedInfo = false;

		public static GridCell currCell;
		public static Stack<Vector2Int> selectedHistory = new Stack<Vector2Int>(HistoryUtils.historyLength);
		public static Stack<Vector2Int> selectedRedoHistory = new Stack<Vector2Int>(HistoryUtils.historyLength);
		static RoadShape roadShape;

		public static Dictionary<KeyCode, RoadShape> shapesDict = new Dictionary<KeyCode, RoadShape>();

		public static Vector2Int mousePos
		{
			get
			{
				Vector3 floatPos = cam.ScreenToWorldPoint(Input.mousePosition);
				return new Vector2Int(Mathf.FloorToInt(Mathf.Clamp(floatPos.x, 0, grid.bounds.x-1)), Mathf.FloorToInt(Mathf.Clamp(floatPos.y, 0, grid.bounds.y-1)));
			}
		}

		void GenerateShapesDict()
		{
			shapesDict.Clear();
			shapesDict.Add(KeyCode.L, new ShapeLine());
			shapesDict.Add(KeyCode.E, new ShapeEraser());
			shapesDict.Add(KeyCode.C, new ShapeCircle());
			shapesDict.Add(KeyCode.A, new ShapeArc());
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
			if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)) //also allow Q, for WebGL
			{
				RoadShape.ClearPreview(grid);
				if(roadShape != null) roadShape.OnKeyUp();
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
			bool ctrlDown = false;
			if(Input.GetKey(KeyCode.LeftControl) || Application.isEditor && Input.GetKey(KeyCode.RightShift)) //Control
			{
				ctrlDown = true;
				if (Input.GetKeyDown(KeyCode.Z))
				{
					grid.Undo();
				}
				if (Input.GetKeyDown(KeyCode.Y))
				{
					grid.Redo();
				}
			}
			else if (Input.GetKey(KeyCode.LeftShift)) //Shift
			{
				if (Input.GetKeyDown(KeyCode.C))
				{
					grid.CleanseGrid();
				}
			}
			else //No modifier key
			{
				if (Input.GetKeyDown(KeyCode.Period))
				{
					CellStateExtensions.showCentreline = !CellStateExtensions.showCentreline;
					grid.RefreshGrid();
				}
				if (Input.GetKeyDown(KeyCode.Comma))
				{
					showAdvancedInfo = !showAdvancedInfo;
					grid.ToggleChunkBorders(showAdvancedInfo);
					grid.RefreshGrid();
				}
			}

			Vector2Int shapeNewStartpoint = new Vector2Int(int.MinValue, int.MinValue);
			if (currCell != null)
			{
				foreach (KeyCode key in shapesDict.Keys)
				{
					if (Input.GetKeyDown(key) && !Input.GetKey(KeyCode.LeftShift) && !ctrlDown)
					{
						roadShape = shapesDict[key];
						roadShape.startPoint = currCell.position;
						roadShape.OnKeyDown();
					}
					if (Input.GetKey(key) && roadShape == shapesDict[key])
					{
						roadShape.OnKeyHeld();
						roadShape.PreviewChanges(grid);

						if(Input.mouseScrollDelta.y > 0.1f || Input.mouseScrollDelta.y < -0.1f)
						{
							RoadShape.thickness = Mathf.Clamp(RoadShape.thickness + (1.0f * Mathf.Sign(Input.mouseScrollDelta.y)), 1.0f, grid.size*CellChunk.chunkSize);
						}
						if(Input.GetMouseButtonDown(0))
						{
							roadShape.ApplyChanges(grid);
							shapeNewStartpoint = roadShape.GetNewStartPoint();
						}
					}
					if (Input.GetKeyUp(key) && roadShape == shapesDict[key])
					{
						RoadShape.ClearPreview(grid);
						roadShape.OnKeyUp();
						roadShape = null;
					}
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				Vector2Int chosenCellPos = mousePos;
				if (shapeNewStartpoint.x != int.MinValue)
					chosenCellPos = shapeNewStartpoint;

				HighlightCell(chosenCellPos, shapeNewStartpoint.x != int.MinValue);
				if(roadShape != null && currCell != null)
					roadShape.startPoint = currCell.position;
			}
		}

		void HandleMenuInput()
		{

		}

		public static void HighlightCell(Vector2Int pos, bool forceOn = false)
		{
			if (!grid.IsInBounds(pos)) return;
	
			//Deselect current cell
			if (currCell != null) currCell.Highlight(false);
			if (currCell == null || currCell.position != pos || forceOn) //we selected a different cell (or we force selecting the cell)
			{
				currCell = grid.GetCellAt(pos.x, pos.y);
				currCell.Highlight(true);
			}
			else //we clicked the already selected cell, so we just wanted to deselect it
			{
				currCell = null;
			}
		}

		public static void SaveToHistory(bool clearRedoHistory = true)
		{
			if (clearRedoHistory) //not called from a Redo(), make sure that tools save their states too
			{
				foreach (RoadShape tool in shapesDict.Values)
				{
					tool.SaveToHistory(clearRedoHistory);
				}
			}

			if (clearRedoHistory)
				selectedRedoHistory.Clear();

			if (selectedHistory.Count == HistoryUtils.historyLength) //history full - remove oldest
			{
				selectedHistory.RemoveOldest();
			}
			if(currCell != null)
				selectedHistory.Push(currCell.position);
		}
		public static void Undo()
		{
			foreach (RoadShape tool in shapesDict.Values)
			{
				tool.OnUndo();
			}

			RoadShape.ClearPreview(grid);
			if (roadShape != null)
			{
				roadShape.OnKeyUp();
				roadShape = null;
			}

			if (selectedHistory.Count > 0)
			{
				if(currCell != null)
					selectedRedoHistory.Push(currCell.position);
				HighlightCell(selectedHistory.Pop(), true);
			}
		}
		public static void Redo()
		{
			foreach (RoadShape tool in shapesDict.Values)
			{
				tool.OnRedo();
			}

			RoadShape.ClearPreview(grid);
			if (roadShape != null)
			{
				roadShape.OnKeyUp();
				roadShape = null;
			}

			if (selectedRedoHistory.Count > 0)
			{
				SaveToHistory(false);
				HighlightCell(selectedRedoHistory.Pop(), true);
			}
		}
	}
}
