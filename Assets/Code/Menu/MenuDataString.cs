using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Code.Menu
{
    public class MenuDataString : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        private void OnEnable()
        {
            UpdateDataString(CellGrid.instance);
        }

        public void UpdateDataString(CellGrid grid)
        {
            Stopwatch timer = Stopwatch.StartNew();
            string dataStr = ImportExportGrid.ExportGrid(grid);
            inputField.text = dataStr;
            timer.Stop();
            Debug.Log($"Grid serialised to base64 string (length={dataStr.Length}) in {timer.Elapsed.TotalMilliseconds}ms.");
        }

        public void ImportButtonPressed()
        {
            string dataStr = inputField.text;
        
            Stopwatch timer = Stopwatch.StartNew();
            bool greatSuccess = ImportExportGrid.ImportGrid(CellGrid.instance, dataStr);
            timer.Stop();
            if(greatSuccess)
                Debug.Log($"Grid imported from base64 string (length={dataStr.Length}) in {timer.Elapsed.TotalMilliseconds}ms.");
        
            PauseMenu menu = GetComponentInParent<PauseMenu>();
            menu.ToggleOpen(false); //refresh menu
            menu.ToggleOpen(true);
        }
    }
}
