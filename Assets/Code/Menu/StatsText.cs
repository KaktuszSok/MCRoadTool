using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsText : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI text;

	public void RefreshStats()
	{
		int road = 0;
		int footpath = 0;

		for (int y = 0; y < CellGrid.instance.bounds.y; y++)
		{
			for (int x = 0; x < CellGrid.instance.bounds.x; x++)
			{
				CellState state = CellGrid.instance.GetCellAt(x, y).state;
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
