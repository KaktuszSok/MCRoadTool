using TMPro;
using UnityEngine;

namespace Code.Menu
{
	public class FramerateCounter : MonoBehaviour
	{
		TextMeshProUGUI text;

		void Awake()
		{
			text = GetComponent<TextMeshProUGUI>();
		}

		void Update()
		{
			text.text = (Time.deltaTime * 1000f).ToString("F1") + "ms";
		}
	}
}
