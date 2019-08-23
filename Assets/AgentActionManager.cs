using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentActionManager : MonoBehaviour
{

	UIActionBarController actionButtons;
	AgentMovement agentMovement;

    // Start is called before the first frame update
    void Start()
    {
		actionButtons = FindObjectOfType<UIActionBarController>();
		agentMovement = GetComponent<AgentMovement>();
    }

	public void AttemptMoveAction()
	{
		// determine if you have enough AP
		agentMovement.MovementActionStarted();
	}
	public void ReportEndOfAction()
	{
		
	}
}
