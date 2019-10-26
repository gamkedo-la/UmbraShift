using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTruck : MonoBehaviour, IInteractable
{
	[Header("Config")]
	[SerializeField] private InteractionScreen noProjectData;
	[SerializeField] private InteractionScreen gameWin;
	[SerializeField] private InteractionScreen enterScene;
	[SerializeField] private Chatbox chatbox;
	[SerializeField] private string levelToGo = "GameWin";
	private PlayerCharacterData playerCharacterData;
	public Item ObjectRequired;

	public void Start()
	{
		chatbox.Open(enterScene);
		playerCharacterData = FindObjectOfType<PlayerCharacterData>();
		if (playerCharacterData) { playerCharacterData.AdvanceStory(); }
	}
	
	public void Interact()
	{
		if (ObjectRequired && Inventory.instance.HasItem(ObjectRequired))
		{
			if (playerCharacterData) { playerCharacterData.AdvanceStory(); }
			if (chatbox) { chatbox.Open(gameWin); }
		}
		else if (ObjectRequired && !Inventory.instance.HasItem(ObjectRequired))
		{
			if (chatbox) { chatbox.Open(noProjectData); }
		}
	}
}
