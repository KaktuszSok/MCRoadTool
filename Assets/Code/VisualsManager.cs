using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
