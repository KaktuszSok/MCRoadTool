using System;
using UnityEngine;

namespace Code
{
	public class CellGrid : MonoBehaviour
	{
		public static CellGrid instance;
		public GameObject chunkPrefab;

		//Settings
		[Tooltip("Height of grid")]
		public int size = 4;
		public float aspectRatio = 16f / 9f;
		public Vector2Int bounds { get; private set; }

		//Data
		public CellChunk[,] chunks = new CellChunk[0, 0];
	
		//Runtime
		public bool showChunkBorders { get; private set; }
		public Action<CellGrid> onMapRegenerated;

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
			bounds = new Vector2Int((int)(size * aspectRatio), size)*CellChunk.chunkSize;
			chunks = new CellChunk[bounds.x/CellChunk.chunkSize, bounds.y/CellChunk.chunkSize];
			for(int y = 0; y*CellChunk.chunkSize < bounds.y; y++)
			{
				for(int x = 0; x*CellChunk.chunkSize < bounds.x; x++)
				{
					CellChunk newChunk = Instantiate(chunkPrefab, transform).GetComponent<CellChunk>();
					newChunk.SetPosition(x, y);
					newChunk.ToggleBorders(showChunkBorders);

					chunks[x, y] = newChunk;
				}
			}
			for(int y = 0; y < chunks.GetLength(1)-1; y++)
			{
				for(int x = 0; x < chunks.GetLength(0); x++)
				{
					chunks[x,y].SetNorthChunk(chunks[x,y+1]);
				}
			}
			onMapRegenerated?.Invoke(this);
		}

		public void ClearGrid()
		{
			for(int y = 0; y < chunks.GetLength(1); y++)
			{
				for(int x = 0; x < chunks.GetLength(0); x++)
				{
					Destroy(chunks[x, y].gameObject);
				}
			}

			PlayerInput.currCell = null;
		}

		public void CleanseGrid()
		{
			SaveToHistory();
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				for (int x = 0; x < chunks.GetLength(0); x++)
				{
					chunks[x, y].CleanseChunk();
				}
			}
		}

		public void RefreshGrid()
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				for (int x = 0; x < chunks.GetLength(0); x++)
				{
					chunks[x, y].UpdateCellColours();
					chunks[x, y].UpdateTexture();
				}
			}
		}

		public void SaveToHistory(bool clearRedoHistory = true)
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				for (int x = 0; x < chunks.GetLength(0); x++)
				{
					chunks[x, y].SaveToHistory(clearRedoHistory);
				}
			}
			PlayerInput.SaveToHistory(clearRedoHistory);
		}
		public void Undo()
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				for (int x = 0; x < chunks.GetLength(0); x++)
				{
					chunks[x, y].Undo();
				}
			}
			PlayerInput.Undo();
		}
		public void Redo()
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				for (int x = 0; x < chunks.GetLength(0); x++)
				{
					chunks[x, y].Redo();
				}
			}
			PlayerInput.Redo();
		}

		public void RealignCam()
		{
			Camera cam = Camera.main;

			Vector3 camPos = cam.transform.position;
			camPos.x = transform.position.x + (bounds.x) / 2f;
			camPos.y = transform.position.y + (bounds.y) / 2f;

			cam.orthographicSize = (size*CellChunk.chunkSize + 1f) / 2f;
			cam.transform.position = camPos;
		}

		public GridCell GetCellAt(int x, int y)
		{
			return chunks[x/CellChunk.chunkSize, y/CellChunk.chunkSize].cells[x % CellChunk.chunkSize, y % CellChunk.chunkSize];
		}

		public bool IsInBounds(Vector2Int pos)
		{
			if (pos.x < 0 || pos.y < 0 || pos.x >= bounds.x || pos.y >= bounds.y)
				return false;
			return true;
		}

		public void ToggleChunkBorders(bool visible)
		{
			showChunkBorders = visible;
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				for (int x = 0; x < chunks.GetLength(0); x++)
				{
					chunks[x, y].ToggleBorders(showChunkBorders);
				}
			}
		}
	}
}
