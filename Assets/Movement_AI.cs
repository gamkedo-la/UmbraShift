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
				Vector3 firePointPosInLocalCoord = new Vector3 (firePoint.transform.position.x, transform.position.y, firePoint.transform.position.z);
				offset = (transform.position - firePointPosInLocalCoord);
			}
			int rangeAsInt = (int)agentStats.EquippedWeapon.range;
			float rangeAsFloat = (float)rangeAsInt;
			float idealRange = rangeAsFloat * 0.4f;

			if (CalcDistanceToPlayer() > idealRange)
			{
				Vector3 vecFromPlayerToTargetPoint = (transform.position - player.transform.position).normalized * (idealRange + offset.magnitude);
				targetPoint = player.transform.position + vecFromPlayerToTargetPoint;
			}

			LayerMask floorLayer = LayerMask.GetMask("Floor");
			bool isPathToTargetPointClear = !(Physics.CheckSphere(targetPoint, CHECKSPHERE_SIZE, ~floorLayer));
			float distToPlayer = CalcDistanceToPlayer();
			float distToTarget = (targetPoint - transform.position).magnitude;
			float maxRnd = 0f;
			int maxLoops = 9;

			do
			{
				maxLoops -= 1;
				if (maxLoops < 1) { targetPoint = transform.position; break; }
				maxRnd += 0.5f;
				float rndNum = Random.Range(0f, maxRnd);
				Vector3 vecFromPlayerToTargetPoint = (transform.position - player.transform.position).normalized * (idealRange + offset.magnitude + rndNum);
				targetPoint = player.transform.position + vecFromPlayerToTargetPoint;
				isPathToTargetPointClear = !(Physics.CheckSphere(targetPoint, CHECKSPHERE_SIZE, ~floorLayer));
			} while (!isPathToTargetPointClear);

			float distanceFromTargetToPlayer = Vector3.Distance(targetPoint, player.transform.position);
			float currentDistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
			bool targetPointIsMyCurrentPosition = Vector3.Distance(transform.position, targetPoint) <= ARRIVAL_THRESHOLD;
			bool targetIsCloserToPlayer = distanceFromTargetToPlayer < currentDistanceToPlayer;

			if (isPathToTargetPointClear && targetIsCloserToPlayer && !targetPointIsMyCurrentPosition)
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
