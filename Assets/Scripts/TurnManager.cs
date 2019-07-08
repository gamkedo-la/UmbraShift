using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    List<PlayerController> playerControllers = new List<PlayerController>();
    List<EnemyController> enemyControllers = new List<EnemyController>();
    
    public bool playersTurn = true;
    public bool isCombatModeActive = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("singleton already existed but attempted re-assignment");
        }
        else
        {
            instance = this;
        }
    }

    public void Start()
    {
        DebugManager.instance.OverwriteDebugText($"Combat mode is {isCombatModeActive}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Enter pressed");
            SwitchTurn();
        }
    }

    public void SwitchTurn()
    {
        
        playersTurn = !playersTurn;
        
        foreach (PlayerController eachPC in playerControllers)
        {
            if (playersTurn)
            {
                BaseCharacterClass tempBase = eachPC.GetComponent<BaseCharacterClass>();
                if (tempBase != null)
                {
                    tempBase.ActionPointRefill();
                }
            }
        }

        foreach (EnemyController eachEnemy in enemyControllers)
        {
            if (!playersTurn)
            {
                BaseCharacterClass tempBase = eachEnemy.GetComponent<BaseCharacterClass>();
                if (tempBase != null)
                {
                    tempBase.ActionPointRefill();
                }
            }
        }
    }

    public void CombatMode()
    {
        isCombatModeActive = !isCombatModeActive;
        DebugManager.instance.OverwriteDebugText($"Combat mode is {isCombatModeActive}");
        Debug.Log($"CombatModeActive = {isCombatModeActive}");
    }

    public void PlayerControllerReportingForDuty(PlayerController playerController)
    {
        playerControllers.Add(playerController);
        foreach (var enemyController in enemyControllers)
        {
            enemyController.CheckForNewPlayerControllers();
        }
    }
    
    public void EnemyControllerReportingForDuty(EnemyController enemyController)
    {
        enemyControllers.Add(enemyController);
        enemyController.CheckForNewPlayerControllers();
    }

    public List<PlayerController> GetCharacterControllers()
    {
        List<PlayerController> characters = new List<PlayerController>();
        foreach (PlayerController eachPC in playerControllers)
        {
            if (eachPC.CompareTag("Player"))
            {
                characters.Add(eachPC);
            }
        }
        return characters;
    }
}
