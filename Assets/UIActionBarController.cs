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
	private bool actionBarHidden=false;
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
		inputManager.MoveButtonPressed += OnMoveButtonPressed;
		skillIconLocations = new Vector3[skillIcons.Length];
		turnManager = FindObjectOfType<GameManager>();
		SetSkillIcons();
		HideActionBar();
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

	protected virtual void OnMoveButtonPressed()
	{
		Debug.Log("Move pressed.");
		if (!playerHasIconControl) { return; }
		int actionPointsRequired = 1;
		AgentStats agent = turnManager.ActiveCharacter;
		if (!resolvingMovement)
		{
			if (agent.SpendActionPoints(actionPointsRequired))
			{
				resolvingMovement = true;
				agent.AgentMovement.ActionUsedForMovement(); 
			}
		}
		else 
		{ 
			agent.AgentMovement.EndMovement();
			agent.AdjustActionPoints(actionPointsRequired);
			resolvingMovement = false;
		}

		//TODO: I'm kind of thinking that the responsibility for attempting to setting and paying action costs
		//should be the responsibility of an agent-based script.  Maybe AgentStats.  
		//Especially since different agents might have different AP costs for the same actions.
		//Maybe we should simply tell that script what is being attempted, and let that script figure out
		//if action points need to be spent or whatever.  
	}

	private void SetSkillIcons()
	{
		for (int i = 0; i < skillIcons.Length; i++)
		{
			skillIconLocations[i] = skillIcons[i].transform.position;
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
		playerHasIconControl = false;
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
		playerHasIconControl = true;
	}

}
