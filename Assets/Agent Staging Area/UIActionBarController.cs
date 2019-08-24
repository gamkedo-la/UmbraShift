using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionBarController : MonoBehaviour
{
	[SerializeField] private Image ActionBarPortrait;
	[SerializeField] private Image ActionBarPortraitFrame;
	[SerializeField] private Button mainButton;
	[SerializeField] private Text activeCharacterName;
	[SerializeField] private GameObject[] skillIcons;
	[SerializeField] private Transform hidingSpot;
	private Vector3[] skillIconLocations;
	private GameManager turnManager;
	private bool actionBarHidden=true;
	private PlayerAgentInput inputManager;
	private float iconDriftTime = 0.5f;
	private bool playerHasIconControl = true;
	private IEnumerator revealingIcon;
	private IEnumerator hidingIcon;
	private bool resolvingMovement = false;
	
	private void Start()
    {
		inputManager = FindObjectOfType<PlayerAgentInput>();
		inputManager.PortraitButtonPressed += OnPortraitButtonPressed;
		inputManager.MoveHotkeyWasPressed += OnMoveHotkeyPressed;
		skillIconLocations = new Vector3[skillIcons.Length];
		turnManager = FindObjectOfType<GameManager>();
		SetSkillIcons();
    }
	
    private void Update()
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

	protected virtual void OnPortraitButtonPressed()
	{
		if (!playerHasIconControl) { return; }
		else
		{
			if (actionBarHidden == true) { RevealActionBar(); }
			else if (actionBarHidden == false) { HideActionBar(); }
		}
	}

	protected virtual void OnMoveHotkeyPressed()
	{
		if (!playerHasIconControl) { return; }
		UIMoveButtonWasPressed();
	}

	public void UIMoveButtonWasPressed()
	{
		if (!playerHasIconControl) { return; }
		inputManager.InitiateMoveAction();
	}

	private void SetSkillIcons()
	{
		for (int i = 0; i < skillIcons.Length; i++)
		{
			skillIconLocations[i] = skillIcons[i].transform.position;
			skillIcons[i].transform.position = hidingSpot.position;
		}
	}

	private void RevealActionBar()
	{
		actionBarHidden = false;
		for (int i = 0; i < skillIcons.Length; i++)
		{
			//skillIcons[i].transform.position = skillIconLocations[i];
			//skillIcons[i].SetActive(true);
			revealingIcon = MoveIcon(skillIcons[i], hidingSpot.position, skillIconLocations[i]);
			StartCoroutine(revealingIcon);

		}
	}

	private void HideActionBar()
	{
		actionBarHidden = true;
		foreach (GameObject skillIcon in skillIcons) 
		{
			//skillIcon.SetActive(false);
			hidingIcon = MoveIcon(skillIcon, skillIcon.transform.position, hidingSpot.position);
			StartCoroutine(hidingIcon);
		}
	}

	private IEnumerator MoveIcon (GameObject icon, Vector3 start, Vector3 finish)
	{
		LockIconControl();
		float counter = 0;
		float duration = iconDriftTime;
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

}
