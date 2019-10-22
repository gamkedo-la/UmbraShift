using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBoss : MonoBehaviour, IInteractable
{

	[SerializeField] private Item lootItem;
	[SerializeField] private InteractionScreen lootScreen;

	public void Interact()
	{
		Inventory inventory = FindObjectOfType<Inventory>();
		inventory.Add(lootItem);
		FindObjectOfType<Chatbox>().Open(lootScreen);
		this.gameObject.SetActive(false);
	}
    
}
