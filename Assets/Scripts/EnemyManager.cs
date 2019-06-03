using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<EnemyController> enemyControllers = new List<EnemyController>();

    public void EnemyControllerReportingForDuty(EnemyController enemyController)
    {
        enemyControllers.Add(enemyController);
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

    // Update is called once per frame
    void Update()
    {
        List<PlayerController> playerControllers = TurnManager.instance.GetCharacterManagers();
//        Debug.Log($"player controllers {playerControllers.Count}");
        
        foreach (EnemyController enemy in enemyControllers)
        { 
            bool playerCanSee = false;
            foreach (PlayerController player in playerControllers)
            {
                Vector3 fromPlayerToNPC = enemy.gameObject.transform.position - player.transform.position;
                RaycastHit rhinfo;
                if (Physics.Raycast(player.transform.position, fromPlayerToNPC, out rhinfo, player.visualRange))
                {
                    Debug.Log(rhinfo.collider.name);
                    if (rhinfo.collider.CompareTag("NPC"))
                    {
                        playerCanSee = true;
                    }
                    else
                    {
                        Debug.Log($"collider tag is {rhinfo.collider.tag}");
                    }
                }
            }
            enemy.PlayerCanSee(playerCanSee);
        }
    }
}
