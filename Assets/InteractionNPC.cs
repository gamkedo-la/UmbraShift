using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNPC : MonoBehaviour, IInteractable
{
	[SerializeField] private InteractionScreen introduction;
	[SerializeField] private InteractionScreen preMissionChatter;
	[SerializeField] private Chatbox chatBox;
	[SerializeField] private bool advanceStory;
	public bool introduced = false;
	private int chats = 0;


	public void Interact()
	{
		if (chatBox && chats == 0)
		{
			chatBox.Open(introduction);
			chats = chats + 1;
			introduced = true;
			if (advanceStory){ FindObjectOfType<PlayerCharacterData>().AdvanceStory(); }
		}
		else if (chatBox && preMissionChatter && chats > 0)
		{
			chatBox.Open(preMissionChatter);
		}
	}
}
