using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentActionManager : MonoBehaviour
{
	private UIActionBarController m_uiActionBarController;
	private AgentMovement m_agentMovement;
	private AgentStats m_agentStats;
	public Action actionInProgress = Action.None;

	//AP costs
	private int transactionInProcessAP = 0;
	private int moveCostAP = 0;                 //temporarily treating Move as costing 0 instead of 1. 
	private int shootCostAP = 0;
	private int hackCostAP = 0;
	private int	interactCostAP = 0;

	void Start()
    {
		m_uiActionBarController = FindObjectOfType<UIActionBarController>();
		m_agentStats = GetComponent<AgentStats>();
		m_agentMovement = GetComponent<AgentMovement>();
	}

	public bool CanActionBePerformed(Action actionAttempted)
	{
		if (actionAttempted == Action.Continue) { return true; }
		if (actionAttempted == Action.Undo) { return true; }
		if (actionAttempted == Action.Cancel) { return true; }
		if (actionAttempted == Action.Move && m_agentStats.CurrentActionPoints >= moveCostAP) { return true; }
		if (actionAttempted == Action.Shoot && m_agentStats.CurrentActionPoints >= shootCostAP) { return true; }
		if (actionAttempted == Action.Hack && m_agentStats.CurrentActionPoints >= hackCostAP) { return true; }
		if (actionAttempted == Action.Interact && m_agentStats.CurrentActionPoints >= interactCostAP) { return true; }
		return false;
	}

	public void ContinueAction ()
	{
		if (actionInProgress == Action.Move) { m_agentMovement.ActionContinue(); }
	}

	public void CancelAction()
	{
		if (actionInProgress==Action.Move)
		{
			m_agentMovement.ActionCancel();
			transactionInProcessAP = 0;
			ReportEndOfAction();
		}
	}

	public void UndoAction()
	{
		if (actionInProgress == Action.Move)
		{
			m_agentMovement.Undo();
		}
	}

	public void PerformAction (Action action)
	{
		if (action==Action.None) { return; }
		if (action == Action.Continue) { ContinueAction(); }
		if (action == Action.Cancel) { CancelAction(); }
		if (action == Action.Undo) { UndoAction(); }

		if (action==Action.Move)
		{
			if (moveCostAP > m_agentStats.CurrentActionPoints) { ReportActionCancelled(); }
			else 
			{
				transactionInProcessAP = moveCostAP;
				m_agentMovement.ActionStarted();
				actionInProgress = Action.Move;
			}
		}
	}
	   	  

	public void ReportActionCancelled()
	{

	}

	public void ReportEndOfAction()
	{
		m_agentStats.AdjustActionPoints(-transactionInProcessAP);
		transactionInProcessAP = 0;
		m_uiActionBarController.ReadyForNextAction();
	}


}
