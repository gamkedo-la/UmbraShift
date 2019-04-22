using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    List<ActionManager> actionManagers = new List<ActionManager>();

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

    // Start is called before the first frame update
    void Start()
    {

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

        foreach (ActionManager eachAM in actionManagers)
        {
            if (eachAM.tag == "NPC" && playersTurn == false)
            {

                eachAM.ResetActionPoints();
            }

            if (eachAM.tag == "Player" && playersTurn == true)
            {
                eachAM.ResetActionPoints();
            }
        }
    }

    public void ActionManagerReportingForDuty(ActionManager manager)
    {
        actionManagers.Add(manager);
    }
}
