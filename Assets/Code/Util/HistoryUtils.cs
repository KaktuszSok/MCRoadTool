using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HistoryUtils
{
	public const int historyLength = 10;

    public static void RemoveOldest<T>(this Stack<T> history)
	{
		if (history.Count == 0) return;

		List<T> temp = new List<T>(history);
		temp.RemoveAt(temp.Count - 1);
		history.Clear();
		for (int i = temp.Count - 1; i >= 0; i--)
		{
			history.Push(temp[i]);
		}
	}
}
