using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
	
	[SerializeField] private InteractionScreen dropInteraction;
	private Chatbox chatboxInExistence;
	static bool warehouseKeyHasBeenDropped = false;
	bool dropWarehouseKey = true;

	private void Start()
	{
		chatboxInExistence = FindObjectOfType<Chatbox>();
	}

	private void OnDisable()
	{
		if (dropWarehouseKey && !warehouseKeyHasBeenDropped && chatboxInExistence) 
		{
			warehouseKeyHasBeenDropped = true;
			chatboxInExistence.Open(dropInteraction);
		}
	}
}
