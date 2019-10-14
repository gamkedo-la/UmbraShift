using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentActionManager : MonoBehaviour
{
	private UIActionBarController m_uiActionBarController;
	private AgentMovement m_agentMovement;
	private AgentShooting m_agentShooting;
	private AgentInteracting m_agentInteracting;
	private AgentStats m_agentStats;
	public Action actionInProgress = Action.None;
	private bool infiniteActionsInScene = false;

    public Material highlightedMaterial;

	//AP costs
	private int transactionInProcessAP = 0;
	private int moveCostAP = 1;
	private int shootCostAP = 1;
	private int hackCostAP = 1;
	private int	interactCostAP = 1;

	void Start()
    {
		m_uiActionBarController = FindObjectOfType<UIActionBarController>();
		m_agentStats = GetComponent<AgentStats>();
		m_agentMovement = GetComponent<AgentMovement>();
		m_agentShooting = GetComponent<AgentShooting>();
		m_agentInteracting = GetComponent<AgentInteracting>();
		if (!AgentTurnManager.instance.turnManagerActiveInScene) { infiniteActionsInScene = true; }
	}

	public bool CanActionBePerformed(Action actionAttempted)
	{
        if (infiniteActionsInScene)
        {
            return true;
        }


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
		if (actionInProgress == Action.Shoot) { m_agentShooting.ActionContinue(); }
		if (actionInProgress == Action.Interact) { m_agentInteracting.ActionContinue(); }
		if (actionInProgress == Action.Shoot) { m_agentShooting.ActionContinue(); }
	}

	public void CancelAction()
	{
		if (actionInProgress==Action.Move)
		{
			m_agentMovement.ActionCancel();
			transactionInProcessAP = 0;
			ReportEndOfAction();
		}

		if (actionInProgress==Action.Shoot)
		{
			m_agentShooting.ActionCancel();
			transactionInProcessAP = 0;
			ReportEndOfAction();
		}

		if (actionInProgress==Action.Interact)
		{
			m_agentInteracting.ActionCancel();
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

		if (actionInProgress == Action.Shoot)
		{
			m_agentShooting.Undo();
		}

		if (actionInProgress == Action.Interact)
		{
			m_agentInteracting.Undo();
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

		if (action==Action.Shoot)
		{
			if (shootCostAP > m_agentStats.CurrentActionPoints) { ReportActionCancelled(); }
			else
			{
				transactionInProcessAP = shootCostAP;
				m_agentShooting.ActionStarted();
				actionInProgress = Action.Shoot;
			}
		}

		if (action==Action.Interact)
		{
			if (interactCostAP > m_agentStats.CurrentActionPoints) { ReportActionCancelled(); }
			else
			{
				transactionInProcessAP = interactCostAP;
				m_agentInteracting.ActionStarted();
				actionInProgress = Action.Interact;
			}
		}
		if (infiniteActionsInScene) { transactionInProcessAP = 0; }
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

	public void Move_AI()
	{
	
	}

	public void Shoot_AI()
	{
	
	}

}
