using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<EnemyController> enemyControllers = new List<EnemyController>();
    public List<PlayerController> playerControllers = new List<PlayerController>();

    public void EnemyControllerReportingForDuty(EnemyController enemyController)
    {
        enemyControllers.Add(enemyController);
    }

    public void ReportEnemySighted(PlayerController playerController)
    {
        if (playerControllers.Contains(playerController) == false)
        {
            Debug.Log($"playerController name {playerController.name}");
            playerControllers.Add(playerController);
        }
    }

    public List<PlayerController> GetVisiblePlayers()
    {
        return playerControllers;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("already an enemy manager instance");
        }
    }

    private void Start()
    {
        StartCoroutine(CheckIfPlayerCanSeeNPCs());
    }

    // Update is called once per frame
    IEnumerator CheckIfPlayerCanSeeNPCs()
    {
        while (true)
        {
            List<PlayerController> playerControllers = TurnManager.instance.GetCharacterControllers();

            foreach (EnemyController enemy in enemyControllers)
            { 
                bool playerCanSee = false;
                foreach (PlayerController player in playerControllers)
                {
                    Vector3 fromPlayerToNPC = enemy.gameObject.transform.position - player.transform.position;
                    RaycastHit rhinfo;
                    if (Physics.Raycast(player.transform.position, fromPlayerToNPC, out rhinfo, player.visualRange))
                    {
//                        Debug.Log(rhinfo.collider.name);
                        if (rhinfo.collider.CompareTag("NPC"))
                        {
                            playerCanSee = true;
                        }
                        else
                        {
//                            Debug.Log($"collider tag is {rhinfo.collider.tag}");
                        }
                    }
                }
                enemy.PlayerCanSee(playerCanSee);
            }

            if (TurnManager.instance.playersTurn == false && TurnManager.instance.isCombatModeActive)
            {
                foreach (var enemy in enemyControllers)
                {
                    BaseCharacterClass bcc = enemy.GetComponent<BaseCharacterClass>();
                    if (bcc.currentAP >= 3)
                    {
                        int randIndex = UnityEngine.Random.Range(0, playerControllers.Count);
                        enemy.SetTarget(playerControllers[randIndex]);
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
