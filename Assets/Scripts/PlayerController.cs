using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Transform target;
    public BaseCharacterClass enemyController;
    public float visualRange = 30.0f;
    public int currentAP;
    
    public int currentHealth;
    private BaseCharacterClass baseClass;
    private Vector3 targetMoveLocation;
    public GameObject muzzleFlash;

    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        TurnManager.instance.PlayerControllerReportingForDuty(this);
        baseClass = GetComponent<BaseCharacterClass>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        currentHealth = baseClass.maxHealth;
        targetMoveLocation = transform.position;
        
        baseClass.ActionPointRefill();
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

    public bool AttemptToSpend(int cost, bool spendIfWeCan)
    {
        return baseClass.AttemptToSpend(cost, spendIfWeCan);
    }

    public void SetTarget(Transform target)
    {
        enemyController = target.GetComponent<BaseCharacterClass>();
        Debug.Log($"set target to {enemyController.name}");
        if (enemyController == null)
        {
            Debug.Log("We clicked on an enemy with no controller");
        }
    }

    public void SingleShot()
    {
        if (enemyController == null)
        {
            Debug.Log("Can't shoot, no enemy selected.");
        }
        else
        {
            Debug.Log($"shooting at {enemyController.name}");
            baseClass.ShootAtTarget(enemyController);
            GameObject tempGO = Instantiate(muzzleFlash);
            tempGO.transform.position = transform.position;
        }
    }
}
