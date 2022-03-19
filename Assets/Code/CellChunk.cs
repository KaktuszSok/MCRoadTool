using Code.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Code
{
	public class CellChunk : MonoBehaviour
	{
		public const int chunkSize = 16;
		private static readonly Color32 heightTint = new Color32(15, 15, 15, 255);

		Texture2D texture;
		SpriteRenderer rend;

		/// <summary>
		/// Position of in chunk co-ordinates
		/// </summary>
		public Vector2Int position;
		public GridCell[,] cells = new GridCell[chunkSize, chunkSize];

		[CanBeNull] private CellChunk northChunk;

		public bool isDirty = false;

		private void Awake()
		{
			texture = new Texture2D(chunkSize, chunkSize, TextureFormat.RGB24, false);
			texture.filterMode = FilterMode.Point;

			rend = GetComponent<SpriteRenderer>();
			rend.sprite = Sprite.Create(texture, new Rect(0, 0, chunkSize, chunkSize), Vector2.zero, 16);

			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					cells[x, y] = new GridCell();
					cells[x, y].Awake(this);
				}
			}
		}

		public void SetNorthChunk(CellChunk northChunk)
		{
			this.northChunk = northChunk;
		}

		private void Update()
		{
			if(isDirty)
			{
				UpdateTexture();
				isDirty = false;
			}
		}

		public void SetPosition(int xpos, int ypos)
		{
			position = new Vector2Int(xpos, ypos);
			transform.position = new Vector3(position.x * chunkSize, position.y * chunkSize, transform.position.z);
			transform.name = "Chunk (" + xpos + "," + ypos + ")";

			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					cells[x, y].SetPosition(xpos * chunkSize + x, ypos * chunkSize + y);
				}
			}
		}

		public void CleanseChunk()
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					cells[x, y].SetState(CellState.EMPTY);
				}
			}
		}

		public void UpdateCellColours()
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					cells[x, y].UpdateColour();
				}
			}
		}

		public void UpdateTexture()
		{
			Color32[] colours = new Color32[chunkSize*chunkSize];
			for(int y = 0; y < chunkSize; y++)
			{
				for(int x = 0; x < chunkSize; x++)
				{
					Color32 cellColour = cells[x, y].colour;
					short height = cells[x, y].height;
					short heightNorth = getHeightNorthOf(x, y);
					if (height < heightNorth)
					{
						cellColour = ColourUtils.subtractClamped(cellColour, heightTint, cellColour.a);
					}
					else if (height > heightNorth)
					{
						cellColour = ColourUtils.addClamped(cellColour, heightTint, cellColour.a);
					}
					colours[y*chunkSize + x] = cellColour;
				}
			}
			texture.SetPixels32(colours);
			texture.Apply();
		}

		/// <summary>
		/// Get the height of the cell directly north of this one
		/// </summary>
		private short getHeightNorthOf(int x, int y)
		{
			if (y < chunkSize-1)
				return cells[x, y + 1].height;
			else if (northChunk)
				return northChunk.cells[x, 0].height;
			else //no north chunk - assume same height
				return cells[x, y].height;
		}

		public void SaveToHistory(bool clearRedoHistory = true)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					cells[x, y].AddCurrentStateToHistory(clearRedoHistory);
				}
			}
		}
		public void Undo()
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					cells[x, y].RecoverPreviousState();
				}
			}
		}
		public void Redo()
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					cells[x, y].RedoUndoneState();
				}
			}
		}

		public void ToggleBorders(bool visible)
		{
			GetComponent<LineRenderer>().enabled = visible;
		}
	}
}
