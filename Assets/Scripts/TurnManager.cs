using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    List<PlayerController> playerControllers = new List<PlayerController>();

    public bool playersTurn = true;

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
            if (eachPC.CompareTag("NPC") && playersTurn == false)
            {

                eachPC.ResetActionPoints();
            }

            if (eachPC.CompareTag("Player") && playersTurn == true)
            {
                eachPC.ResetActionPoints();
            }
        }
    }

    public void PlayerControllerReportingForDuty(PlayerController playerController)
    {
        playerControllers.Add(playerController);
    }

    public List<PlayerController> GetCharacterManagers()
    {
        List<PlayerController> characters = new List<PlayerController>();
        foreach (PlayerController eachPC in playerControllers)
        {
            if (eachPC.CompareTag("Player") && playersTurn == true)
            {
                characters.Add(eachPC);
            }
        }
        return characters;
    }
}
