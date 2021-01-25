using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public bool isOpen { get; private set; }

	public void ToggleOpen(bool open)
	{
		isOpen = open;
		gameObject.SetActive(open);

		if (open)
			GetComponentInChildren<StatsText>().RefreshStats();
	}
}
