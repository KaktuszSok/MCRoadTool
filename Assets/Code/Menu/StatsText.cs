using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsText : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI text;

	public void RefreshStats()
	{
		GridCell[,] cells = CellGrid.instance.cells;

		int road = 0;
		int footpath = 0;

		for (int y = 0; y < cells.GetLength(1); y++)
		{
			for (int x = 0; x < cells.GetLength(0); x++)
			{
				CellState state = cells[x, y].state;
				if (state == CellState.ROAD || state == CellState.CENTRELINE)
					road++;
				else if (state == CellState.FOOTPATH)
					footpath++;
			}
		}

		text.text = "Map Size: " + CellGrid.instance.bounds.x + "x" + CellGrid.instance.bounds.y +
			"\nRoad Blocks: " + road +
			"\nFootpath Blocks: " + footpath;
	}
}
