using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTest : MonoBehaviour, IInteractable
{
	MeshRenderer rend;
	[SerializeField] Material material1;
	[SerializeField] Material material2;

	private void Start()
	{
		rend = GetComponentInChildren<MeshRenderer>();
	}

	public void Interact()
	{
		Debug.Log("Interaction successful.");
		if (rend.sharedMaterial != material1) { rend.sharedMaterial = material1; }
		else { rend.sharedMaterial = material2; }
		
	}
    
}
