using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public bool isOpen { get; private set; }

	private void Awake()
	{
		StartCoroutine(FixLayout());
	}

	IEnumerator FixLayout()
	{
		yield return new WaitForEndOfFrame();

		gameObject.SetActive(false);
		gameObject.SetActive(true);
	}

	public void ToggleOpen(bool open)
	{
		isOpen = open;
		if (open)
		{
			GetComponentInChildren<StatsText>(true).RefreshStats();
		}
		gameObject.SetActive(open);
	}
}
