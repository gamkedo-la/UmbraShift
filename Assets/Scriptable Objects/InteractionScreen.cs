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
	public string goesToLevel;
	[HideInInspector]public int id;
	public static int totalOptions=0;

	public UIoption(string _optionText, InteractionScreen _optionLeadsTo, bool _lastInteraction)
	{
		id = totalOptions;
		totalOptions = totalOptions + 1;
		optionText = _optionText;
		optionLeadsTo = _optionLeadsTo;
		lastInteraction = _lastInteraction;
		goesToLevel = "";
	}
	public UIoption(string _optionText, InteractionScreen _optionLeadsTo, bool _lastInteraction, string _goesToLevel)
	{
		id = totalOptions;
		totalOptions = totalOptions + 1;
		optionText = _optionText;
		optionLeadsTo = _optionLeadsTo;
		lastInteraction = _lastInteraction;
		goesToLevel = _goesToLevel;
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

    public Inventory inventory;
    public Item itemToReceive;
}
