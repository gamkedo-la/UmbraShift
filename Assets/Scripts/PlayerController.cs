using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Transform target;
    public float visualRange = 30.0f;
    public int currentAP;
    
    public int currentHealth;
    private BaseCharacterClass baseClass;
    private Vector3 targetMoveLocation;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        baseClass = GetComponent<BaseCharacterClass>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        currentHealth = baseClass.maxHealth;
        targetMoveLocation = transform.position;
        
        ResetActionPoints();
        TurnManager.instance.PlayerControllerReportingForDuty(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (this == InputManager.instance.currentPlayerController)
        {
            targetMoveLocation = target.position;
            Vector3 travelDiff = targetMoveLocation - transform.position;
            travelDiff.y = 0.0f;

            Vector3 moveDiff = agent.velocity;
            moveDiff.y = 0.0f;

            if (travelDiff.magnitude > 0.5 && moveDiff.magnitude > 0.1)
            {
                transform.rotation = Quaternion.LookRotation(moveDiff);
            }

            
            agent.SetDestination(targetMoveLocation);
        }

        CheckLineOfSite();
    }

    public void SetAsActivePlayerController()
    {
        target.position = targetMoveLocation;
    }
    
    public void ResetActionPoints()
    {
        Debug.Log("Resetting action points");
        currentAP = baseClass.ActionPointRefill();
    }

    public bool AttemptToSpend(int cost, bool spendIfWeCan)
    {
        if (cost <= currentAP)
        {
            if (spendIfWeCan)
            {
                currentAP -= cost;
                Debug.Log($"Spent {cost} and have {currentAP} remaining.");
            }
            return true;
        }
        Debug.Log("Couldn't afford so didn't remove cost");
        return false;
    }

    void CheckLineOfSite()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, visualRange);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("NPC"))
            {
                //Debug.Log("NPC is in the overlapsphere");
                Vector3 rayFromMeToNPC = hitColliders[i].gameObject.transform.position - transform.position;
                RaycastHit rhinfo;
                if (Physics.Raycast(transform.position, rayFromMeToNPC, out rhinfo, visualRange))
                {
                    //Debug.Log(rhinfo.collider.name);
                    if (rhinfo.collider.CompareTag("NPC"))
                    {
                        EnemyController enemyCtrl = hitColliders[i].GetComponent<EnemyController>();
                        if (enemyCtrl != null)
                        {
                            enemyCtrl.PlayerCanSee(true);
                        }
                    }
                    else
                    {
                        EnemyController enemyCtrl = hitColliders[i].GetComponent<EnemyController>();
                        if (enemyCtrl != null)
                        {
                            enemyCtrl.PlayerCanSee(false);
                        }
                    }
                }
            }
        }
    }
}
