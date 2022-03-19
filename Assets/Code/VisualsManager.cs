using UnityEngine;

namespace Code
{
	/// <summary>
	/// Handles extra visual effects and provides prefab references
	/// </summary>
	public class VisualsManager : MonoBehaviour
	{
		public static VisualsManager instance;

		public GameObject tangentLinePrefab;

		private void Awake()
		{
			instance = this;
		}
	}
}
