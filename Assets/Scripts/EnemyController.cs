using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    public Transform patrolPointOne;
    public Transform patrolPointTwo;
    public int pauseTime = 3;
    public Text noticeIndicator;
    public float visualRange = 20.0f;

    public float FOV = 90.0f;
    public bool moving = false;

    public int currentHealth;
    private NavMeshAgent agent;
    private BaseCharacterClass baseClass;
    public List<PlayerController> playerControllers;
    public List<PlayerController> visiblePlayers = new List<PlayerController>();
    public PlayerController playerTarget;

    // Start is called before the first frame update
    void Start()
    {
        TurnManager.instance.EnemyControllerReportingForDuty(this);
        target = patrolPointOne;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        moving = true;
        baseClass = GetComponent<BaseCharacterClass>();
        currentHealth = baseClass.maxHealth;
        EnemyManager.instance.EnemyControllerReportingForDuty(this);
        baseClass.ActionPointRefill();
    }

	void OnDisable( )
	{
        TurnManager.instance?.EnemyControllerReportingOffDuty(this);
        EnemyManager.instance?.EnemyControllerReportingOffDuty(this);
	}

	// Update is called once per frame
	void Update()
    {
        if (TurnManager.instance.isCombatModeActive)
        {
            target.position = transform.position;
            visiblePlayers = EnemyManager.instance.GetVisiblePlayers();
        }

        if (moving)
        {
            Vector3 travelDiff = target.position - transform.position;
            travelDiff.y = 0.0f;

            Vector3 moveDiff = agent.velocity;
            moveDiff.y = 0.0f;

            if (travelDiff.magnitude > 0.5 && moveDiff.magnitude > 0.1)
            {
                transform.rotation = Quaternion.LookRotation(moveDiff);
            }
            agent.SetDestination(target.position);
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance && TurnManager.instance.isCombatModeActive == false)
                {
                    moving = false;
                    if (target == patrolPointOne)
                    {
                        target = patrolPointTwo;
                    }
                    else
                    {
                        target = patrolPointOne;
                    }
                    StartCoroutine(PauseForTime());
                }
            }
        }
        noticeIndicator.text = "?";
        foreach (PlayerController eachPC in playerControllers)
        {
            Vector3 rayFromMeToPlayer = eachPC.transform.position - transform.position;
            float degreesNeededToFacePlayer = Quaternion.Angle(transform.rotation,
            Quaternion.LookRotation(rayFromMeToPlayer));
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(FOV / 2, Vector3.up) * transform.forward * 5.0f, Color.red, 0, true);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(FOV / -2, Vector3.up) * transform.forward * 5.0f, Color.red, 0, true);
            if (degreesNeededToFacePlayer < FOV / 2)
            {

                // add distance check before raycast
                RaycastHit rhinfo;
                if (Physics.Raycast(transform.position, rayFromMeToPlayer, out rhinfo, visualRange))
                {
//                    Debug.Log($"collider name is {rhinfo.collider.name}");
//                    Debug.Log($"rhinfo layer is {rhinfo.transform.gameObject.layer}");
                    if (rhinfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        noticeIndicator.text = rhinfo.collider.name;
                        Debug.Log($"{name} is reporting {eachPC.name} as seen");
                        EnemyManager.instance.ReportEnemySighted(eachPC);
                        if (TurnManager.instance.isCombatModeActive == false)
                        {
                            TurnManager.instance.CombatMode();
                        }
                    }
                }
            }
        }
    }

    public void SetTarget(PlayerController player)
    {
        playerTarget = player;
    }

    public void CheckForNewPlayerControllers()
    {
        playerControllers = TurnManager.instance.GetCharacterControllers();
    }

    public bool AttemptToSpend(int cost, bool spendIfWeCan)
    {
        return baseClass.AttemptToSpend(cost, spendIfWeCan);
    }

    public void PlayerCanSee(bool isVisible)
    {
        if (isVisible)
        {
            RecursiveLayerMaskSet("VisibleNPC", transform);
        }
        else
        {
            RecursiveLayerMaskSet("HiddenNPC", transform);
        }
    }

    void RecursiveLayerMaskSet(string layerName, Transform whichTransform)
    {
        whichTransform.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in whichTransform)
        {
            RecursiveLayerMaskSet(layerName, child);
        }
    }

    IEnumerator PauseForTime()
    {
        yield return new WaitForSeconds(pauseTime);
        moving = true;
    }
}
