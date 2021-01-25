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
		int size = 100;
		string sizeInput = newMapSizeField.text;
		int.TryParse(sizeInput, out size);
		size = Mathf.Clamp(size, 15, 200);

		CellGrid.instance.size = size;
		CellGrid.instance.RegenerateGrid();
		CellGrid.instance.RealignCam();

		PlayerInput.instance.menu.ToggleOpen(true);
	}
}
