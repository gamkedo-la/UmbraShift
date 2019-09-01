using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHotkeyInput : MonoBehaviour
{
	private UIActionBarController actionBarController;
	private bool inputLocked = false;
	public bool InputLocked { get { return inputLocked; } set { inputLocked = value; } }
	
	public void LockInput(bool locking)
	{
		inputLocked = locking;
	}
	
	private void Start()
	{
		actionBarController = FindObjectOfType<UIActionBarController>();
	}

	private void Update()
	{
	
			if (Input.GetKeyDown(KeyCode.M)) 
			{
				actionBarController.OnMoveActionButtonPressed();
			} 
			else if (Input.GetKeyDown(KeyCode.S))
			{
				actionBarController.OnShootActionButtonPressed();
			}
			else if (Input.GetKeyDown(KeyCode.H))
			{
				actionBarController.OnHackActionButtonPressed();
			}
			else if (Input.GetKeyDown(KeyCode.I))
			{
				actionBarController.OnInteractActionButtonPressed();
			}
			else if (Input.GetKeyDown(KeyCode.Z))
			{
				actionBarController.OnUndoActionButtonPressed();
			}
			else if (Input.GetKeyDown(KeyCode.X))
			{
				actionBarController.OnCancelActionButtonPressed();
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				actionBarController.OnContinueActionButtonPressed();
			}

	}
}
