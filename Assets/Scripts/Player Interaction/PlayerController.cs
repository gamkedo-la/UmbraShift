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
    public float maxMoveWithAvailableAP;
    public float movePerAP = 1f;

    public int currentHealth;
    private BaseCharacterClass baseClass;
    private Vector3 targetMoveLocation;
    public GameObject muzzleFlash;
    public GameObject playerSelectIndicator;
    public NavMeshAgent agent;

    public Hackable hackableObject;
    public canBeInvestigated objectBeingInvestigated;
	public bool isMainCharacter = false;

    public int multiShotAccuracyPenalty = -15;

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

    void OnDisable()
    {
        TurnManager.instance?.PlayerControllerReportingOffDuty(this);
    }

    // Update is called once per frame
    void Update()
    {
        maxMoveWithAvailableAP = currentAP * movePerAP;
        if (this.gameObject == TurnManager.instance.ActiveCharacter)
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
        ActivateCharacter();
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

    public void MultiShot(int numberOfShots)
    {
        //WIP
        if (enemyController == null)
        {
            Debug.Log("Can't shoot, no enemy selected.");
        }
        else
        {
            Debug.Log($"shooting at {enemyController.name}");

            for (int i = 0; i < (numberOfShots); i++)
            {
                baseClass.ShootAtTarget(enemyController);
                //GameObject tempGO = Instantiate(muzzleFlash);
                //tempGO.transform.position = transform.position;
                baseClass.shooting.AddModifier(multiShotAccuracyPenalty);                
            }

            for (int i = 0; i < (numberOfShots); i++)
            {
                baseClass.shooting.RemoveModifier(multiShotAccuracyPenalty);                
            }
        }
    }

    public void SetHackTarget(Transform target)
    {
        hackableObject = target.GetComponent<Hackable>();
        Debug.Log($"set hacking target to {hackableObject.name}");
        if (hackableObject == null)
        {
            Debug.Log("Not a hackable object");
        }
    }

    public void Hack()
    {
        if (hackableObject == null)
        {
            Debug.Log("Can't be hacked");
        }
        else
        {
            Debug.Log("hacking " + hackableObject.name);
            baseClass.AttemptHack(hackableObject);
        }
    }

    public void SetInvestigationTarget(Transform target)
    {
        objectBeingInvestigated = target.GetComponent<canBeInvestigated>();
        Debug.Log("Taking a closer look at " + objectBeingInvestigated.name);

        if (objectBeingInvestigated == null)
        {
            Debug.Log("Cannot understand object further.");
        }
    }

    public void Investigate()
    {
        if (objectBeingInvestigated == null)
        {
            Debug.Log("Can't be investigated");
        }
        else
        {
            Debug.Log("investigating" + objectBeingInvestigated.name);
            baseClass.AttemptInvestigation(objectBeingInvestigated);
        }
    }

    private void ToggleSelectionIndicator(bool selectionIndicatorOn)
    {
        playerSelectIndicator.SetActive(selectionIndicatorOn);
    }

    public void ActivateCharacter()
    {
        ToggleSelectionIndicator(true);
    }

    public void DeactivateCharacter()
    {
        ToggleSelectionIndicator(false);
    }





}
