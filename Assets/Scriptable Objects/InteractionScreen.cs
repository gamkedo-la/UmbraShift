using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UIoption
{
	public string optionText;
	public InteractionScreen optionLeadsTo;
	public bool lastInteraction;
	[HideInInspector]public int id;
	public static int totalOptions=0;

	public UIoption(string _optionText, InteractionScreen _optionLeadsTo, bool _lastInteraction)
	{
		id = totalOptions;
		totalOptions = totalOptions + 1;
		optionText = _optionText;
		optionLeadsTo = _optionLeadsTo;
		lastInteraction = _lastInteraction;
	}
}

[CreateAssetMenu(fileName = "New Interaction Screen", menuName = "Interaction Screen")]
public class InteractionScreen : ScriptableObject
{
	public Sprite portrait;
	public string handle;
	[Space]
	[TextArea]public string description;
	[Space]
	public Color color;
	[Space]	[Space]	[Space]
	public UIoption[] options;
}
