using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTruck : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private InteractionScreen noProjectData;
	[SerializeField] private InteractionScreen gameWin;
	[SerializeField] private Chatbox chatbox;
	[SerializeField] private string levelToGo = "GameWin";
	public Item ObjectRequired;

	public void Interact()
	{
		if (ObjectRequired && Inventory.instance.HasItem(ObjectRequired))
		{
			if (chatbox) { chatbox.Open(gameWin); }
		}
		else if (!ObjectRequired)
		{
			if (chatbox) { chatbox.Open(noProjectData); }
		}
	}
}
