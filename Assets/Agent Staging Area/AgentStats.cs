using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{

	// float movementPoints;
	// float actionPoints;
	[SerializeField] private Sprite portraitImage;
	[SerializeField] private string characterName = "DefaultName";
	private AgentMovement agentMovement;
	private int currentActionPoints = 0;
	private int maxActionPoints = 2;
	private int movementPointsPerAction = 20;
	public int MovementPointsPerAction { get { return movementPointsPerAction; } }

	public AgentMovement AgentMovement { get { return agentMovement; } }
	public int CurrentActionPoints { get { return currentActionPoints; } }
	public Sprite PortraitImage { get { return portraitImage; } }
	public string CharacterName { get { return characterName; } }
	
	
	void Start()
	{
		agentMovement = GetComponent<AgentMovement>();
		currentActionPoints = maxActionPoints;
	}


	public Sprite GetPortraitImage() { return portraitImage; }
	public string GetCharacterName() { return characterName; }
	public AgentMovement GetAgentMovement() { return agentMovement; }
	public int GetCurrentActionPoints() { return currentActionPoints; }
	public int GetMovementPointsPerAction()	{ return movementPointsPerAction; }
	
	public bool SpendActionPoints(int apCost)
	{
		if (apCost <= currentActionPoints)
		{
			AdjustActionPoints(-apCost);
			return true;
		}
		else { return false; }
	}

	public void AdjustActionPoints (int amt)
	{
		currentActionPoints = currentActionPoints + amt;
	}
	
    
}
