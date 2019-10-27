using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour, IInteractable
{
	[SerializeField] private InteractionScreen lootScreen;
	[SerializeField] private InteractionScreen alreadyLooted;
	private bool lootRemaining = true;

	public void Interact()
	{
		Chatbox chatbox = FindObjectOfType<Chatbox>();
		if (lootRemaining)
		{
			if (chatbox && lootScreen) { chatbox.Open(lootScreen); }
			PlayerCharacterData playerCharacterData = FindObjectOfType<PlayerCharacterData>();
			if (playerCharacterData) { playerCharacterData.AdvanceStory(); }
		}
		else
		{
			if (!lootRemaining && chatbox && alreadyLooted) { chatbox.Open(alreadyLooted); }
		}
	}

}
