using Code.Shapes;
using TMPro;
using UnityEngine;

namespace Code.Menu
{
	public class MenuSettings : MonoBehaviour
	{
		private const float defaultAspectRatio = 16f / 9f;
		public TMP_InputField newMapSizeField;

		private void Start()
		{
			CellGrid.instance.onMapRegenerated += OnMapRegenerated;
		}

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
			size = Mathf.Clamp(size, 1, 18);
			newMapSizeField.text = size.ToString();

			CellGrid.instance.size = size;
			CellGrid.instance.aspectRatio = defaultAspectRatio;
			CellGrid.instance.RegenerateGrid();
			CellGrid.instance.RealignCam();

			PlayerInput.instance.menu.ToggleOpen(true);
		}

		public void OnMapRegenerated(CellGrid grid)
		{
			newMapSizeField.text = grid.size.ToString();
		}
	}
}
