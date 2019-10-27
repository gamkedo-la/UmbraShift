using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement_AI : MonoBehaviour
{
	AgentStats player;
	AgentStats agentStats;
	AgentActionManager actionManager;
	NavMeshAgent navMesh;
	Senses_AI senses;
	List<NavPoint> allNavPoints = new List<NavPoint>();
	bool movementSystemActive = false;
	bool isMoving=false;
	float movementLimit;
	const float ARRIVAL_THRESHOLD = 0.2f;
	private enum Activity { Patrol, Alert, None }
	private Activity activity = Activity.None;

    private void Start()
    {
		player = FindObjectOfType<PlayerHotkeyInput>().gameObject.GetComponent<AgentStats>();
		agentStats = GetComponent<AgentStats>();
		navMesh = GetComponent<NavMeshAgent>();
		actionManager = GetComponent<AgentActionManager>();
		senses = GetComponent<Senses_AI>();
		NavPoint[] allNavPointsArray = FindObjectsOfType<NavPoint>();
		foreach (NavPoint navPoint in allNavPointsArray) { allNavPoints.Add(navPoint); }
		movementLimit = agentStats.GetMovementPointsPerAction();
    }

    private void Update()
    {
		if (activity ==Activity.Alert) {AlertUpdate(); }
		else if (activity==Activity.Patrol) { PatrolUpdate(); }
    }

	private float CalcDistanceToPlayer()
	{
		if (player)
		{
			return Vector3.Distance(transform.position, player.transform.position);
		}
		else { return 0f; }
	}

	private void AlertUpdate()
	{
		if (!isMoving)
		{
			Vector3 targetPoint = transform.position;
			if (CalcDistanceToPlayer() > (float)agentStats.EquippedWeapon.range / 2f)
			{
				Vector3 vecTotargetPoint = (transform.position - player.transform.position).normalized * (float)agentStats.EquippedWeapon.range / 2f;
				targetPoint = player.transform.position + vecTotargetPoint;
			}
			if (Vector3.Distance(transform.position, targetPoint) > movementLimit)
			{
				targetPoint = (targetPoint - transform.position).normalized * movementLimit;
			}
			LayerMask floorLayer = LayerMask.GetMask("Floor");
			bool isPathToTargetPointClear = Physics.CheckSphere(targetPoint, 0.1f, -floorLayer);
			if (isPathToTargetPointClear && Vector3.Distance(transform.position, targetPoint) > ARRIVAL_THRESHOLD)
			{
				navMesh.SetDestination(targetPoint);
			}
			else
			{
				navMesh.SetDestination(transform.position);
				ActionEnded();
			}
		}
		else 
		{ 
			if (Vector3.Distance(transform.position, navMesh.destination) <= ARRIVAL_THRESHOLD)
			{
				isMoving = false;
				navMesh.destination = transform.position;
				ActionEnded();
			}
		}
	}

	private void PatrolUpdate()
	{
		
	}

	public void ActionStarted()
	{
		movementSystemActive = true;
		if (movementSystemActive && senses)
		{
			if (senses.GetAlertStatus() == AlertStatus.OnAlert) { activity = Activity.Alert; }
			if (senses.GetAlertStatus() == AlertStatus.OnPatrol) { activity = Activity.Patrol; }
		}
	}

	private void ActionEnded()
	{
		activity = Activity.None;
		movementSystemActive = false;
		actionManager.ReportEndOfMoving_AI();
	}
}
