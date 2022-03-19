using UnityEngine;
using UnityEngine.UI;

namespace Code.Menu
{
	/// <summary>
	/// Makes a (vertical) ScrollView shrink if its contents dont fill all of its height
	/// </summary>
	[ExecuteAlways]
	[RequireComponent(typeof(LayoutElement))]
	public class ScrollviewSizeLimiter : MonoBehaviour
	{
		LayoutElement scrollLayout;
		public RectTransform content;
		Image bgImage;
		public float bgAlpha;

		public float maxHeight = 175f;

		private void Awake()
		{
			scrollLayout = GetComponent<LayoutElement>();
			bgImage = GetComponent<Image>();
		}

		private void Update()
		{
			if (content && content.sizeDelta.y <= maxHeight)
			{
				if(Mathf.Approximately(scrollLayout.preferredHeight, content.sizeDelta.y))
				{
					return;
				}
				scrollLayout.preferredHeight = content.sizeDelta.y;
				if (bgImage)
					bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, 0f);
			}
			else if (scrollLayout.preferredHeight != maxHeight)
			{
				scrollLayout.preferredHeight = maxHeight;
				if(bgImage)
					bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, bgAlpha);
			}
		}
	}
}
