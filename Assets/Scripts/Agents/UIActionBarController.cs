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
	[SerializeField] private UIIcon undoActionIcon;
	[SerializeField] private UIIcon continueActionIcon;
	[SerializeField] private UIIcon cancelActionIcon;
	[SerializeField] private UIIcon moveActionIcon;
	[SerializeField] private UIIcon shootActionIcon;
	[SerializeField] private UIIcon interactActionIcon;
	[SerializeField] private UIIcon hackActionIcon;
	[SerializeField] private Button endTurnButton;

	[SerializeField] private Transform hidingSpot;
	[SerializeField] private Transform[] homeSpot;
	[SerializeField] private float iconMoveSpeed = 20f;

	private UIIcon iconInMemory = null;
	private UIIcon[] allIcons;
	private AgentTurnManager turnManager;
	private PlayerHotkeyInput inputManager;
	private bool playerHasIconControl = true;
	private IEnumerator movingIcons;
	
	
	private void Start()
    {
		allIcons = FindObjectsOfType<UIIcon>();
		inputManager = FindObjectOfType<PlayerHotkeyInput>();
		turnManager = FindObjectOfType<AgentTurnManager>();
		SetInitialIconPositions();
		FindObjectOfType<GridSelectionController>().RightClick += OnUndoActionButtonPressed;
		UpdatePortraitInfo();
		StartCoroutine("DelayedUpdate");
	}

	private IEnumerator DelayedUpdate()
	{
		yield return new WaitForSeconds(1f);
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
		UpdatePortraitInfo();       // TODO: pause game and show character info window
		DelayedUpdate();
	}

	private IEnumerator MoveIcon(UIIcon icon, Vector3 start, Vector3 finish)
	{
		LockIconControl();
		float counter = 0;
		float delay = icon.GetFlashDelay();
		float duration = (icon.driftTime / iconMoveSpeed) + delay;
		Vector3 currentPosition = start;
		while (counter < delay)
		{
			counter = counter + Time.deltaTime;
			yield return null;
		}
		counter = 0;
		while (counter < duration)
		{
			counter = counter + Time.deltaTime;
			currentPosition = Vector3.Lerp(start, finish, counter / duration);
			icon.transform.position = currentPosition;
			yield return null;
		}
		ResetIconControl();
	}

	private void HideActionIcon(UIIcon icon, bool tryingToHide)
	{
		if (tryingToHide && !icon.hidden) 
		{
            FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.UISwish);
			movingIcons = MoveIcon(icon, icon.homePosition, icon.hidePosition);
			icon.hidden = true;
		}
		else if (!tryingToHide && icon.hidden) 
		{
            FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.UISwish);
            movingIcons = MoveIcon(icon, icon.hidePosition, icon.homePosition);
			icon.hidden = false;
		}
		else return;
		StartCoroutine(movingIcons);
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
		if (!moveActionIcon.hidden && playerHasIconControl && !inputManager.InputLocked)
		{ AttemptAction(Action.Move, true, false); }
	}

	public void OnShootActionButtonPressed()
	{
		if (!shootActionIcon.hidden && playerHasIconControl && !inputManager.InputLocked)
		{ AttemptAction(Action.Shoot, true, false); }
	}

	public void OnInteractActionButtonPressed()
	{
		if (!interactActionIcon.hidden && playerHasIconControl && !inputManager.InputLocked)
		{ AttemptAction(Action.Interact, true, false); }
	}

	public void OnHackActionButtonPressed()
	{
		if (!hackActionIcon.hidden && playerHasIconControl && !inputManager.InputLocked)
		{ AttemptAction(Action.Hack, true, false); }
	}

	public void OnContinueActionButtonPressed()
	{
		if (!continueActionIcon.hidden && playerHasIconControl && !inputManager.InputLocked)
		{ AttemptAction(Action.Continue, true, false); }
	}

	public void OnCancelActionButtonPressed()
	{
		if (!cancelActionIcon.hidden && playerHasIconControl && !inputManager.InputLocked)
		{ AttemptAction(Action.Cancel, false, true); }
	}

	public void OnUndoActionButtonPressed()
	{
		if (!undoActionIcon.hidden && playerHasIconControl && !inputManager.InputLocked)
		{ AttemptAction(Action.Undo, true, false); }
	}

	public void ReadyForNextAction()
	{
		if (endTurnButton) { endTurnButton.interactable = true; }
		inputManager.InputLocked = false;
		playerHasIconControl = true;
		HideActionIcon(cancelActionIcon, true);
		HideActionIcon(continueActionIcon, true);
		HideActionIcon(undoActionIcon, true);
		foreach (UIIcon iconInSet in actionIconSet) { HideActionIcon(iconInSet, false); }
		turnManager.ActiveCharacter.actionManager.actionInProgress = Action.None;
	}



	private void AttemptAction(Action action, bool hideActionSet, bool hideActionControlsSet)
	{
		if (turnManager.ActiveCharacter==null) { return; }
		playerHasIconControl = false;
		float actionDelay = 0f;
		foreach (UIIcon icon in allIcons)
		{
			if (icon.ActionWhenPressed == action)
			{
                FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.UISelected);
				actionDelay = icon.GetFlashDelay();
				if (turnManager.ActiveCharacter.actionManager.CanActionBePerformed(action))
				{
					icon.Flash(Color.green);
					if (hideActionSet==true) 
					{
						foreach (UIIcon iconInSet in actionIconSet) { HideActionIcon(iconInSet, true); }
					}
					else 
					{
						foreach (UIIcon iconInSet in actionIconSet) { HideActionIcon(iconInSet, false); }
					}
					if (hideActionControlsSet==true)
					{
						HideActionIcon(cancelActionIcon, true);
						HideActionIcon(undoActionIcon, true);
						HideActionIcon(continueActionIcon, true);
					}
					else 
					{
						HideActionIcon(cancelActionIcon, false);
						HideActionIcon(undoActionIcon, false);
						HideActionIcon(continueActionIcon, false);
					}
				}
				else icon.Flash(Color.red);
				break;
			}
		}
		StartCoroutine(BeginAction(action, actionDelay));
	}

	private IEnumerator BeginAction(Action action, float delay)
	{
		yield return new WaitForSeconds(delay*2);
		playerHasIconControl = true;
		if (endTurnButton) { endTurnButton.interactable = false; }
		turnManager.ActiveCharacter.actionManager.PerformAction(action);
	}

	public void HideEndTurnButton()
	{
		if (endTurnButton) { endTurnButton.interactable = false; }
	}

	public void ShowEndTurnButton()
	{
		if (endTurnButton) { endTurnButton.interactable = true; }
	}


	
}
