using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionBarController : MonoBehaviour
{
	[SerializeField] private Image ActionBarPortrait;
	[SerializeField] private Image ActionBarPortraitFrame;
	[SerializeField] private Text activeCharacterName;
	[SerializeField] private UIIcon[] actionIconSet;
	[SerializeField] private UIIcon rewindActionIcon;
	[SerializeField] private UIIcon playActionIcon;
	[SerializeField] private UIIcon stopActionIcon;

	[SerializeField] private Transform hidingSpot;
	[SerializeField] private Transform[] homeSpot;

	private UIIcon iconInMemory = null;
	private AgentTurnManager turnManager;
	private PlayerAgentInput inputManager;
	private AgentActionManager actionManager;
	private bool playerHasIconControl = true;
	private IEnumerator movingIcons;
	
	private void Start()
    {
		inputManager = FindObjectOfType<PlayerAgentInput>();
		turnManager = FindObjectOfType<AgentTurnManager>();
		actionManager = FindObjectOfType<AgentActionManager>();
		SetInitialIconPositions();
		UpdatePortraitInfo();
	}

	private void SetInitialIconPositions()
	{
		UIIcon[] icons = FindObjectsOfType<UIIcon>();
		foreach (UIIcon icon in icons) 
		{
			icon.SetHidePosition(hidingSpot.position);
			icon.SetHomePosition(homeSpot[icon.ActionIconNum-1].position);
			if (icon.hidden == true) { icon.transform.position = icon.hidePosition; }
			else if (icon.hidden == false) { icon.transform.position = icon.homePosition; }
		}
	}


	public bool IconsAreBusy()
	{
		return !playerHasIconControl;
	}
	
    public void UpdatePortraitInfo()
    {
		if (ActionBarPortrait && turnManager && turnManager.ActiveCharacter && turnManager.ActiveCharacter.PortraitImage) 
		{ 
			ActionBarPortrait.sprite = turnManager.ActiveCharacter.PortraitImage;			
		}

		if (activeCharacterName && turnManager && turnManager.ActiveCharacter)
		{
			 activeCharacterName.text = turnManager.ActiveCharacter.CharacterName;
		}
	}

	public void OnPortraitButtonPressed()
	{
		UpdatePortraitInfo();		// TODO: pause game and show character info window
	}

	private void HideActionIcon(UIIcon icon, bool hiding)
	{
		if (hiding == true && icon.hidden == false) 
		{ 
			movingIcons = MoveIcon(icon, icon.homePosition, icon.hidePosition);
			icon.hidden = true;
		}
		else if (hiding == false && icon.hidden == true) 
		{ 
			movingIcons = MoveIcon(icon, icon.hidePosition, icon.homePosition);
			icon.hidden = false;
		}
		else return;
		StartCoroutine(movingIcons);
	}
	

	private IEnumerator MoveIcon (UIIcon icon, Vector3 start, Vector3 finish)
	{
		LockIconControl();
		float counter = 0;
		float duration = icon.driftTime;
		Vector3 currentPosition = start;
		while (counter<duration)
		{
			counter = counter + Time.deltaTime;
			currentPosition = Vector3.Lerp(start, finish, counter / duration);
			icon.transform.position = currentPosition;
			yield return null;
		}
		ResetIconControl();
	}

	public void ResetIconControl() 
	{
		playerHasIconControl = true;
	}

	public void LockIconControl()
	{
		playerHasIconControl = false;
	}

	public void OnMoveActionButtonPressed() 
	{
		if (playerHasIconControl && !inputManager.InputLocked)
		{
			Debug.Log("Move" + " button was pressed.");
			foreach (UIIcon icon in actionIconSet) { HideActionIcon(icon, true); }
			HideActionIcon(stopActionIcon, false);
			HideActionIcon(rewindActionIcon, false);
			HideActionIcon(playActionIcon, false);
		}
	}

	public void OnShootActionButtonPressed()
	{
		if (playerHasIconControl && !inputManager.InputLocked)
		{
			Debug.Log("Shoot" + " button was pressed.");
			foreach (UIIcon icon in actionIconSet) { HideActionIcon(icon, true); }
			HideActionIcon(stopActionIcon, false);
			HideActionIcon(rewindActionIcon, false);
			HideActionIcon(playActionIcon, false);
		}
	}

	public void OnInteractActionButtonPressed()
	{
		if (playerHasIconControl && !inputManager.InputLocked)
		{
			Debug.Log("Interact" + " button was pressed.");
			foreach (UIIcon icon in actionIconSet) { HideActionIcon(icon, true); }
			HideActionIcon(playActionIcon, false);
		}
	}

	public void OnHackActionButtonPressed()
	{
		if (playerHasIconControl && !inputManager.InputLocked)
		{
			Debug.Log("Hack" + " button was pressed.");
			foreach (UIIcon icon in actionIconSet) { HideActionIcon(icon, true); }
			HideActionIcon(stopActionIcon, false);
			HideActionIcon(rewindActionIcon, false);
			HideActionIcon(playActionIcon, false);
		}
	}

	public void OnStopButtonPressed()
	{
		if (playerHasIconControl && !inputManager.InputLocked)
		{
			Debug.Log("Stop" + " button was pressed.");
			foreach (UIIcon icon in actionIconSet) { HideActionIcon(icon, false); }
			HideActionIcon(stopActionIcon, true);
			HideActionIcon(rewindActionIcon, true);
			HideActionIcon(playActionIcon, true);
		}
	}

	public void OnPlayButtonPressed()
	{
		if (playerHasIconControl && !inputManager.InputLocked)
		{
			Debug.Log("Play" + " button was pressed.");
			foreach (UIIcon icon in actionIconSet) { HideActionIcon(icon, false); }
			HideActionIcon(stopActionIcon, true);
			HideActionIcon(rewindActionIcon, true);
			HideActionIcon(playActionIcon, true);
		}
	}

	public void OnRewindButtonPressed()
	{
		if (playerHasIconControl && !inputManager.InputLocked)
		{
			Debug.Log("Rewind" + " button was pressed.");
			foreach (UIIcon icon in actionIconSet) { HideActionIcon(icon, false); }
			HideActionIcon(stopActionIcon, true);
			HideActionIcon(rewindActionIcon, true);
			HideActionIcon(playActionIcon, true);
		}
	}
}
