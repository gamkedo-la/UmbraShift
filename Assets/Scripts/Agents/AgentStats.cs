using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStats : MonoBehaviour, IComparable<AgentStats>
{
	[Header("Character Info")]
	[SerializeField] public bool isNPC = true;
    [SerializeField] public bool isHuman = true;
    [SerializeField] private Sprite portraitImage;
	public Sprite PortraitImage { get { return portraitImage; } }
	[SerializeField] private string characterName = "Shadow";
	public string CharacterName { get { return characterName; } }

	[Header("Equipment")]
	[SerializeField] private WeaponDesign equippedWeapon;
	[SerializeField] private WeaponDesign defaultWeapon;
	public WeaponDesign EquippedWeapon { get { return equippedWeapon; } }

	[Header("Actions")]
	private int currentActionPoints = 0;
	public int CurrentActionPoints { get { return currentActionPoints; } }
	private int maxActionPoints = 2;
	public AgentActionManager actionManager;

	[Header("Movement")]
	private AgentMovement agentMovement;
	public AgentMovement AgentMovement { get { return agentMovement; } }
	[SerializeField] private float movementAnimSpeed = 7f;
	public float MovementAnimSpeed { get { return movementAnimSpeed; } }
	private int movementPointsPerAction = 6;
	public int MovementPointsPerAction { get { return movementPointsPerAction; } }

	[Header("Stats")]
	public Stat Strength = new Stat();
	public Stat Dexterity = new Stat();
	public Stat Intellect = new Stat();
	public Stat Hacking = new Stat();
	public Stat Shooting = new Stat();
	public Stat Investigation = new Stat();
	public Stat Medicine = new Stat();
	public Stat FastTalking = new Stat();

	[Header("Derived Values")]
	
	public Stat coverBonus = new Stat();
	public Stat coverBypass = new Stat();
	public Stat accuracyBonus = new Stat();
	public Stat damageBonus = new Stat();
	public Stat healingBonus = new Stat();
	public Stat initiativeBonus = new Stat();
	public Stat movementSpeedBonus = new Stat();
	public Stat hitpoints = new Stat();
	public Stat maxHitpoints = new Stat();
	private float hitpointPercentage = 1;
	public float HitpointPercentage { get { return hitpointPercentage; } }
	public int CurrentHitpoints { get { return hitpoints.GetValue(); } }
	public int MaxHitpoints { get { return maxHitpoints.GetValue(); } }
	public int MoveSpeedBonus { get { return movementSpeedBonus.GetValue(); } }

	[Header("Graphics and Animation")]
	private AgentLocalUI localUI;
	


	
	void Start()
	{
		agentMovement = GetComponent<AgentMovement>();
		actionManager = GetComponent<AgentActionManager>();
		localUI = GetComponent<AgentLocalUI>();
		if (defaultWeapon && !equippedWeapon) { equippedWeapon = defaultWeapon; }
		currentActionPoints = maxActionPoints;
		StartCoroutine("DelayedUpdate");
	}

	public void CalculateDerivedValuesFromBaseStats()
	{
		//STR = hitpoints, DEX = movement, INT = cover bonus
		coverBonus.WriteBaseValue(2 * Intellect.GetValue());
		coverBypass.WriteBaseValue(5 * Shooting.GetValue());
		accuracyBonus.WriteBaseValue(10 * Shooting.GetValue());
		//healingBonus.WriteBaseValue(3 * Intellect.GetValue());
		initiativeBonus.WriteBaseValue(1 * Dexterity.GetValue());
		damageBonus.WriteBaseValue((int)(0.5 * (float)Medicine.GetValue()));
		movementSpeedBonus.WriteBaseValue(2 * Dexterity.GetValue());
		maxHitpoints.WriteBaseValue(15);
		maxHitpoints.AddModifier(5 * Strength.GetValue());
		
		hitpoints.WriteBaseValue(maxHitpoints.GetValue());
		hitpointPercentage = (float)hitpoints.GetValue() / (float)maxHitpoints.GetValue();

		movementPointsPerAction += movementSpeedBonus.GetValue();

	}

	public int CompareTo (AgentStats otherAgent)
	{
		//this method is called by AgentTurnManager to compare the speed of different agents to this agent
		if (!otherAgent) { return 1; }
		return initiativeBonus.GetValue() - otherAgent.initiativeBonus.GetValue();
	}

	private IEnumerator DelayedUpdate()
	{
		yield return new WaitForSeconds(0.2f);
		if (isNPC == false)
		{
			PlayerCharacterData playerCharacterData = FindObjectOfType<PlayerCharacterData>();
			if (playerCharacterData)
			{
				if (playerCharacterData.playerName != "") { characterName = playerCharacterData.playerName; }
				portraitImage = playerCharacterData.playerPortrait.sprite;
			}
		}
	}

	public Sprite GetPortraitImage() { return portraitImage; }
	public string GetCharacterName() { return characterName; }
	public AgentMovement GetAgentMovement() { return agentMovement; }
	public int GetCurrentActionPoints() { return currentActionPoints; }
	public int GetMovementPointsPerAction() { return movementPointsPerAction; }

	public void AdjustActionPoints (int amt)
	{
		currentActionPoints = currentActionPoints + amt;
	}

	public void ResetActionPoints()
	{
		currentActionPoints = maxActionPoints;
	}
	
    public void TakeDamage (int dmg)
	{
		hitpoints.AddModifier(-dmg);
		hitpointPercentage = (float)hitpoints.GetValue() / (float)maxHitpoints.GetValue();
	}
}
