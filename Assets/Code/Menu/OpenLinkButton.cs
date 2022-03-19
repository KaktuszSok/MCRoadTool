using UnityEngine;

namespace Code.Menu
{
	public class OpenLinkButton : MonoBehaviour
	{
		public string link = "";

		public void OpenLink()
		{
			Application.OpenURL(link);
		}
	}
}
