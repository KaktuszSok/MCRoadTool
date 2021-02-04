using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuSettings : MonoBehaviour
{
	public TMP_InputField newMapSizeField;

	public void SetFootpathWidth(string input)
	{
		float width = 2.0f;
		float.TryParse(input, out width);
		RoadShape.footpathWidth = width;
	}

	public void CreateNewMap()
	{
		int size = 4;
		string sizeInput = newMapSizeField.text;
		int.TryParse(sizeInput, out size);
		size = Mathf.Clamp(size, 1, 16);
		newMapSizeField.text = size.ToString();

		CellGrid.instance.size = size;
		CellGrid.instance.RegenerateGrid();
		CellGrid.instance.RealignCam();

		PlayerInput.instance.menu.ToggleOpen(true);
	}
}
