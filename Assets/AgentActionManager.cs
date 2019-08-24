using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentActionManager : MonoBehaviour
{
	PlayerAgentInput m_playerInput;
	UIActionBarController m_actionButtons;
	AgentMovement m_agentMovement;
	AgentStats m_agentStats;

	//AP costs
	private int transactionInProcessAP = 0;
	private int moveCostAP = 0;					//temporarily treating Move as costing 0 instead of 1. 
   
    void Start()
    {
		m_playerInput = FindObjectOfType<PlayerAgentInput>();
		m_actionButtons = FindObjectOfType<UIActionBarController>();
		m_agentStats = GetComponent<AgentStats>();
		m_agentMovement = GetComponent<AgentMovement>();
		//m_playerInput.MoveButtonPressed += OnMoveActionSelected;
	}

	public void OnMoveActionSelected()
	{
		if (m_agentStats.CurrentActionPoints >= moveCostAP) 
		{
			transactionInProcessAP = moveCostAP;
			m_agentMovement.MovementActionStarted();
		}
		else 
		{
			ReportActionCancelled();
		}
	}

	public void ReportActionCancelled()
	{
		transactionInProcessAP = 0;
		m_playerInput.actionInProcess = ActionInProcess.None;
	}

	public void ReportEndOfAction()
	{
		m_agentStats.SpendActionPoints(transactionInProcessAP);
		transactionInProcessAP = 0;
		m_playerInput.actionInProcess = ActionInProcess.None;
	}


}
