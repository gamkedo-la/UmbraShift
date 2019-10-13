using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AgentTurnManager : MonoBehaviour
{
	[Header("Scene Setup")]
	[SerializeField] private AgentStats mainPlayerCharacter = null;
	[SerializeField] public bool turnManagerActiveInScene = false;
	[SerializeField] private TurnManagerUI turnUI;
	private AgentStats activeCharacter = null;
	public AgentStats ActiveCharacter { get { return activeCharacter; } }
	private List<AgentStats> agents;
	private int turnIndex = 0;
	private const float ENDTURN_DELAY = 0.35f;
    public static AgentTurnManager instance;

	public AgentStats GetActiveCharacter()
	{
		return activeCharacter;
	}

    void Start()
    {

        if (instance == null)
        {
            instance = this;
        }

        if (turnUI != null)
        {
            turnUI.DeActivateEndTurnButton();
        }
		if (turnManagerActiveInScene == false) 
		{ 
			activeCharacter = mainPlayerCharacter; 
			return; 
		}
		agents = GenerateAgentList();
		turnUI.RegisterCharacters(agents);
		turnIndex = 0;
		ProcessTurn(agents[turnIndex]);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.End)) { EndTurn(); }
	}

	private List<AgentStats> GenerateAgentList()
	{
		AgentStats[] agentArray = FindObjectsOfType<AgentStats>();
		List<AgentStats> agentList = new List<AgentStats>();
		foreach (AgentStats agent in agentArray) { agentList.Add(agent); }
		agentList.Sort();
		foreach (AgentStats agent in agentList) { Debug.Log(agent.name + ", Speed: " + agent.initiativeBonus.GetValue()); }
		return agentList;
	}

	public void ProcessTurn(AgentStats character)
	{
		turnUI.ActivateTurnIndicator(turnIndex);
		activeCharacter = character;
		if (character.isNPC==false) 
		{
			character.ResetActionPoints();	
		}
		else 
		{
			AgentActionManager actionManager = character.gameObject.GetComponent<AgentActionManager>();	
			if (actionManager)
			{
				actionManager.Move_AI();
				actionManager.Shoot_AI();
				StartCoroutine("BeginEndingTurn");
			}
		}
	}

	private IEnumerator BeginEndingTurn()
	{
		yield return new WaitForSeconds(ENDTURN_DELAY);
		EndTurn();
	}

	public void EndTurn ()
	{
		turnUI.DeActivateEndTurnButton();
		turnIndex = turnIndex + 1;
		if (turnIndex >= agents.Count) { turnIndex = 0; }
		ProcessTurn(agents[turnIndex]);
	}




}
