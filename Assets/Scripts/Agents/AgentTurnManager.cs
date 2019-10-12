using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AgentTurnManager : MonoBehaviour
{
	[SerializeField] private AgentStats mainPlayerCharacter = null;
	[SerializeField] private bool turnManagerActiveInScene = false;
	private AgentStats activeCharacter = null;
	public AgentStats ActiveCharacter { get { return activeCharacter; } }
	private List<AgentStats> agents;

	public AgentStats GetActiveCharacter()
	{
		return activeCharacter;
	}

    void Start()
    {
		if (turnManagerActiveInScene == false) 
		{ 
			activeCharacter = mainPlayerCharacter; 
			return; 
		}
		agents = GenerateAgentList();
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
}
