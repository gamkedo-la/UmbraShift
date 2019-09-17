﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{

	// float movementPoints;
	// float actionPoints;
	[SerializeField] private Sprite portraitImage;
	[SerializeField] private string characterName = "DefaultName";
	[SerializeField] private WeaponDesign equippedWeapon;
	[SerializeField] private WeaponDesign defaultWeapon;
	private AgentMovement agentMovement;
	private int currentActionPoints = 0;
	private int maxActionPoints = 2;

	private int movementPointsPerAction = 20;
	public int MovementPointsPerAction { get { return movementPointsPerAction; } }
	[SerializeField] private float movementSpeed = 7f;
	public float MovementSpeed { get { return movementSpeed; } }
	public WeaponDesign EquippedWeapon { get{ return equippedWeapon; } }

	public AgentMovement AgentMovement { get { return agentMovement; } }
	public int CurrentActionPoints { get { return currentActionPoints; } }
	public Sprite PortraitImage { get { return portraitImage; } }
	public string CharacterName { get { return characterName; } }

	public AgentActionManager actionManager;


	public Stat Strength = new Stat();
	public Stat Dexterity = new Stat();
	public Stat Intellect = new Stat();
	public Stat Hacking = new Stat();
	public Stat Shooting = new Stat();
	public Stat Investigation = new Stat();
	public Stat Medicine = new Stat();
	public Stat FastTalking = new Stat();


	
	
	void Start()
	{
		agentMovement = GetComponent<AgentMovement>();
		actionManager = GetComponent<AgentActionManager>();
		currentActionPoints = maxActionPoints;
		StartCoroutine("DelayedUpdate");
		DetermineInitialStats();
		if (defaultWeapon && !equippedWeapon) { equippedWeapon = defaultWeapon; }
	}

	private void DetermineInitialStats()
	{
		// fill stats from character generation, if applicable
		
	}

	private IEnumerator DelayedUpdate()
	{
		yield return new WaitForSeconds(0.5f);
		InitialStatManager initialStatManager = FindObjectOfType<InitialStatManager>();
		if (initialStatManager)
		{
			characterName = initialStatManager.playerName;
			portraitImage = initialStatManager.playerPortrait.sprite;
		}
	}


	public Sprite GetPortraitImage() { return portraitImage; }
	public string GetCharacterName() { return characterName; }
	public AgentMovement GetAgentMovement() { return agentMovement; }
	public int GetCurrentActionPoints() { return currentActionPoints; }
	public int GetMovementPointsPerAction()	{ return movementPointsPerAction; }
	
	public void AdjustActionPoints (int amt)
	{
		currentActionPoints = currentActionPoints + amt;
	}
	
    
}
