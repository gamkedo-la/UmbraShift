using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionScreen introduction;
	[SerializeField] private InteractionScreen chatter;
	[SerializeField] private Chatbox chatBox;
	private int chats = 0;

	public void Interact()
    {
		if (chatBox && chats>0) { chatBox.Open(chatter); }
		else if (chatBox && chats == 0) { chatBox.Open(introduction); }
	}
}
