using System;
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

    private void Awake()
    {
        TurnManager.instance.PlayerControllerReportingForDuty(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        baseClass = GetComponent<BaseCharacterClass>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        currentHealth = baseClass.maxHealth;
        targetMoveLocation = transform.position;
        
        ResetActionPoints();
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
}
