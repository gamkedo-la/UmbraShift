using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement_AI : MonoBehaviour
{
	[SerializeField] private bool STATIC_MOVE = false;
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
    public Animator animator;
	private const float CHECKSPHERE_SIZE = 0.2f;

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
		if (activity == Activity.Alert) {AlertUpdate(); }
		else if (activity==Activity.Patrol) { PatrolUpdate(); }

       if (navMesh.velocity.sqrMagnitude > 0.1f && !animator.GetBool("isWalking"))
        {
            animator.SetBool("isWalking", true);
        }
        else if(animator.GetBool("isWalking"))
        {
            animator.SetBool("isWalking", false);
        }
           
       
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
		if (navMesh.destination==transform.position) { isMoving = false; }
		if (isMoving==false)
		{
			Vector3 targetPoint = transform.position;
			Vector3 offset = Vector3.zero;
			FirePoint firePoint = GetComponentInChildren<FirePoint>();
			if (firePoint) 
			{ 
				Vector3 temp = new Vector3 (firePoint.transform.position.x, transform.position.y, firePoint.transform.position.z);
				offset = (transform.position - temp);
			}
			int rangeAsInt = (int)agentStats.EquippedWeapon.range;
			float rangeAsFloat = (float)rangeAsInt;
			if (CalcDistanceToPlayer() > (rangeAsFloat / 2f))
			{
				Vector3 vecFromPlayerToTargetPoint = (transform.position - player.transform.position).normalized * ((rangeAsFloat / 2f) + offset.magnitude);
				targetPoint = player.transform.position + vecFromPlayerToTargetPoint;
			}
			if (Vector3.Distance(transform.position, targetPoint) > movementLimit)
			{
				Vector3 vecTotargetPoint = (targetPoint - transform.position).normalized * movementLimit;
				targetPoint = transform.position + vecTotargetPoint;
			}
			LayerMask floorLayer = LayerMask.GetMask("Floor");
			bool isPathToTargetPointClear = !(Physics.CheckSphere(targetPoint, CHECKSPHERE_SIZE, ~floorLayer));

			float distToPlayer = CalcDistanceToPlayer();
			float distToTarget = (targetPoint - transform.position).magnitude;
			if (!isPathToTargetPointClear && distToTarget < distToPlayer)
			{
				Vector3 vecPastTargetPoint = (targetPoint - transform.position).normalized * (movementLimit * 1.25f);
				Vector3 newTarget = transform.position + vecPastTargetPoint;
				float distanceToPlayerFromTarget = Vector3.Distance(newTarget, player.transform.position);
				float distanceToTargetFromHere = Vector3.Distance(transform.position, newTarget);
				float distanceToPlayerFromHere = Vector3.Distance(transform.position, player.transform.position);
				if (distanceToTargetFromHere < distanceToPlayerFromHere && distanceToPlayerFromTarget < distanceToPlayerFromHere)
				{
					targetPoint = transform.position + vecPastTargetPoint;
					distToTarget = (targetPoint - transform.position).magnitude;
					isPathToTargetPointClear = !(Physics.CheckSphere(targetPoint, CHECKSPHERE_SIZE, ~floorLayer));
				}
			}
			if (!isPathToTargetPointClear && distToTarget < distToPlayer)
			{
				Vector3 altVecToTargetPoint = (targetPoint - transform.position).normalized * (movementLimit * 0.75f);
				Vector3 newTarget = transform.position + altVecToTargetPoint;
				float distanceToPlayerFromHere = Vector3.Distance(transform.position, player.transform.position);
				float distanceToPlayerFromTarget = Vector3.Distance(newTarget, player.transform.position);
				if (distanceToPlayerFromTarget < distanceToPlayerFromHere) 
				{
					targetPoint = transform.position + altVecToTargetPoint;
					distToTarget = (targetPoint - transform.position).magnitude;
					isPathToTargetPointClear = !(Physics.CheckSphere(targetPoint, CHECKSPHERE_SIZE, ~floorLayer));
				}
			}
			if (!isPathToTargetPointClear && distToTarget < distToPlayer)
			{
				Vector3 nextAltVecToTargetPoint = (targetPoint - transform.position).normalized * (movementLimit * 0.5f);
				Vector3 newTarget = transform.position + nextAltVecToTargetPoint;
				float distanceToPlayerFromHere = Vector3.Distance(transform.position, player.transform.position);
				float distanceToPlayerFromTarget = Vector3.Distance(newTarget, player.transform.position);
				if (distanceToPlayerFromTarget < distanceToPlayerFromHere)
				{
					targetPoint = transform.position + nextAltVecToTargetPoint;
					distToTarget = (targetPoint - transform.position).magnitude;
					isPathToTargetPointClear = !(Physics.CheckSphere(targetPoint, CHECKSPHERE_SIZE, ~floorLayer));
				}
			}
			if (isPathToTargetPointClear && Vector3.Distance(transform.position, targetPoint) > ARRIVAL_THRESHOLD)
			{
				navMesh.SetDestination(targetPoint);
				isMoving = true;
			}
			else
			{
				navMesh.SetDestination(transform.position);
				isMoving = false;
				ActionEnded();
			}
		}
		else if (isMoving)
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
		ActionEnded();
	}

	public void ActionStarted()
	{
		if (STATIC_MOVE == false)
		{
			isMoving = false;
			movementSystemActive = true;
			if (movementSystemActive && senses)
			{
				if (senses.GetAlertStatus() == AlertStatus.OnAlert) { activity = Activity.Alert; }
				if (senses.GetAlertStatus() == AlertStatus.OnPatrol) { activity = Activity.Patrol; }
			}
			else
			{
				activity = Activity.Patrol;
			}
		}
		else { ActionEnded(); }
	}

	private void ActionEnded()
	{
		isMoving = false;
		activity = Activity.None;
		navMesh.destination = transform.position;
		movementSystemActive = false;
		actionManager.ReportEndOfMoving_AI();
	}
}
