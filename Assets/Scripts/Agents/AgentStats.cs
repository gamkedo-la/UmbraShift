using System.Collections;
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
	private AgentLocalUI localUI;


	public Stat Strength = new Stat();
	public Stat Dexterity = new Stat();
	public Stat Intellect = new Stat();
	public Stat Hacking = new Stat();
	public Stat Shooting = new Stat();
	public Stat Investigation = new Stat();
	public Stat Medicine = new Stat();
	public Stat FastTalking = new Stat();

	[SerializeField] private int baseHitpoints = 15;
	[SerializeField] private int bonusHitpointsPerStrStat = 5;
	private int currentHitpoints = 50;
	public int CurrentHitpoints { get { return currentHitpoints; } }
	private int maxHitpoints = 50;
	public int MaxHitpoints { get { return maxHitpoints; } }
	private float hitpointPercentage = 1;
	public float HitpointPercentage { get{ return hitpointPercentage; } }
	
	void Start()
	{
		agentMovement = GetComponent<AgentMovement>();
		actionManager = GetComponent<AgentActionManager>();
		localUI = GetComponent<AgentLocalUI>();
		if (defaultWeapon && !equippedWeapon) { equippedWeapon = defaultWeapon; }
		currentActionPoints = maxActionPoints;
		DetermineInitialStats();
		StartCoroutine("DelayedUpdate");
	}

	private void DetermineInitialStats()
	{
		CalculateHitpoints();
		// fill stats from character generation, if applicable
	}

	private void CalculateHitpoints()
	{
		maxHitpoints = baseHitpoints + (Strength.GetValue() * bonusHitpointsPerStrStat);
		currentHitpoints = maxHitpoints;
		hitpointPercentage = currentHitpoints / maxHitpoints;
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
	public int GetMovementPointsPerAction() { return movementPointsPerAction; }

	public void AdjustActionPoints (int amt)
	{
		currentActionPoints = currentActionPoints + amt;
	}
	
    public void TakeDamage (int dmg)
	{
		currentHitpoints = currentHitpoints - dmg;
		hitpointPercentage = (float)currentHitpoints / (float)maxHitpoints;
	}
}
