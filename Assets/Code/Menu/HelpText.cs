using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class HelpText : MonoBehaviour
{
	TextMeshProUGUI text;

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}
	void Start()
    {
		text.text = ""
		+ GetToolsString() + "\n"
		+ "<u>Controls:</u>\n"
		+ "\tPress left-click to select cell\n"
		+ "\tHold underlined key (as above) to select tool\n"
		+ "\tPress left-click to use tool\n"
		+ "\tHold right-click for alternate mode\n"
		+ "\tCtrl+Z - undo (max " + HistoryUtils.historyLength + ")\n"
		+ "\tCtrl+Y - redo\n"
		+ "\t. - toggle centreline\n"
		+ "\t, - toggle advanced tooltips\n"
		+ "\nMCRoadTool v" + Application.version + " by KaktuszSok";
	}

	string GetToolsString()
	{
		string text = "Tools: ";
		bool first = true;
		foreach(KeyCode key in PlayerInput.shapesDict.Keys)
		{
			if (first)
				first = false;
			else
				text += ", ";

			text += UnderlineLetter(PlayerInput.shapesDict[key].GetName(), key.ToString());
		}

		return text;
	}

	/// <summary>
	/// Can actually underline more than a single letter
	/// </summary>
	string UnderlineLetter(string word, string letter)
	{
		Regex replace = new Regex(Regex.Escape(letter));
		return replace.Replace(word, "<u>" + letter + "</u>", 1);
	}
}
