using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AgentTurnManager : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private UIActionBarController actionBarController;
	[SerializeField] private AgentStats mainPlayerCharacter = null;
	[SerializeField] public bool turnManagerActiveInScene = false;
	[SerializeField] private TurnManagerUI turnUI;
	[Header("Reference Only - Do Not Modify in Unity")]
	[SerializeField] private List<AgentStats> agents;
	private AgentStats activeCharacter = null;
	public AgentStats ActiveCharacter { get { return activeCharacter; } }
	private int turnIndex = 0;
	private const float ENDTURN_DELAY = 0.35f;
    public static AgentTurnManager instance;

	public AgentStats GetActiveCharacter()
	{
		return activeCharacter;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
	void Start()
    {

        if (turnUI != null)
        {
            turnUI.DeActivateEndTurnButton();
        }
        else
        {
            Debug.Log("turnUI is null");
        }
		if (turnManagerActiveInScene == false) 
		{ 
			activeCharacter = mainPlayerCharacter; 
			return; 
		}
		agents = GenerateAgentList();
		turnUI.RegisterCharacters(agents);
		turnIndex = 0;
        //ProcessTurn(agents[turnIndex]);
        StartCoroutine(DelayedStart());
	}

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2f);
        ProcessTurn(agents[0]);


    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.End)) { EndTurn(); }
	}

	private List<AgentStats> GenerateAgentList()
	{
		AgentStats[] agentArray = FindObjectsOfType<AgentStats>();
		List<AgentStats> agentList = new List<AgentStats>();
		foreach (AgentStats agent in agentArray) { agentList.Add(agent); }
		agentList.Sort();
		foreach (AgentStats agent in agentList) { Debug.Log(agent.name + ", Speed: " + agent.initiativeBonus.GetValue()); }
		return agentList;
	}

	public void ProcessTurn(AgentStats character)
	{
		turnUI.ActivateTurnIndicator(turnIndex);
        if (character != null)
        {
            activeCharacter = character;
            if (character.isNPC == false)
            {
                character.ResetActionPoints();
				actionBarController.ResetIconControl();
            }
            else
            {
				actionBarController.LockIconControl();
				AgentActionManager actionManager = character.gameObject.GetComponent<AgentActionManager>();
                if (actionManager)
                {
                    actionManager.Move_AI();
					ReportEndOfMovement(actionManager);
                }
            }
        }
        else
        {
            Debug.Log("character is null calling BeginEndingTurn");
            StartCoroutine("BeginEndingTurn");
        }
	}

	public void ReportEndOfMovement(AgentActionManager actionManager)
	{
		actionManager.Shoot_AI();
	}

	public void ReportEndOfShooting(AgentActionManager actionManager)
	{
		StartCoroutine("BeginEndingTurn");
	}

	private IEnumerator BeginEndingTurn()
	{
		yield return new WaitForSeconds(ENDTURN_DELAY);
		EndTurn();
	}

	public void EndTurn ()
	{
		turnUI.DeActivateEndTurnButton();
		turnIndex = turnIndex + 1;
		agents = GenerateAgentList();
		turnUI.RegisterCharacters(agents);
		if (turnIndex >= agents.Count) { turnIndex = 0; }
		ProcessTurn(agents[turnIndex]);
	}




}
