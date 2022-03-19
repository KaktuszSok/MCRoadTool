using TMPro;
using UnityEngine;

namespace Code
{
	public class MouseTooltip : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI text;

		/// <summary>
		/// Extra text to draw this frame (clears automatically every frame)
		/// </summary>
		public static string extraFrameText = "";

		void LateUpdate()
		{
			transform.position = Input.mousePosition;
			text.text = "(" + PlayerInput.mousePos.x + "," + PlayerInput.mousePos.y + ")\n";
			if(extraFrameText != "")
				text.text += extraFrameText;

			extraFrameText = "";
		}
	}
}
