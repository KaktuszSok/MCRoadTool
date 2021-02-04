using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellChunk : MonoBehaviour
{
	public const int chunkSize = 16;

	Texture2D texture;
	SpriteRenderer rend;

	/// <summary>
	/// Position of in chunk co-ordinates
	/// </summary>
	public Vector2Int position;
	public GridCell[,] cells = new GridCell[chunkSize, chunkSize];

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
				colours[y*chunkSize + x] = cells[x,y].colour;
			}
		}
		texture.SetPixels32(colours);
		texture.Apply();
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
}
