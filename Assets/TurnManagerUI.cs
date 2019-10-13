using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManagerUI : MonoBehaviour
{
	[Header("Scene Setup")]
	[SerializeField] private GameObject layoutGO;
	[SerializeField] private GameObject indicatorPrefab;
	[SerializeField] private Text actionText;
	[SerializeField] private GameObject endTurnIcon;
	private List<AgentStats> charactersInTurnOrder = new List<AgentStats>();
	private List<IndicatorUI> indicators = new List<IndicatorUI>();
	int index = -1;
	bool textActive = false;

	private void Update()
	{
		if (textActive) 
		{
			if (charactersInTurnOrder[index].isNPC==false)
			{
				endTurnIcon.SetActive(true);
				int turns = charactersInTurnOrder[index].CurrentActionPoints;
				if (turns>0) 
				{
					actionText.color = Color.green;
					actionText.text = turns.ToString() + " turns available";
				}
				else
				{
					actionText.color = Color.red;
					actionText.text = "0 turns available";
				}
			}
		}
		else 
		{
			actionText.color = Color.red;
			actionText.text = "Waiting For Turn";
		}
	}

	public void RegisterCharacters(List<AgentStats> agents)
	{
		foreach (AgentStats agent in agents)
		{
			index = index + 1;
			charactersInTurnOrder.Add(agent);
			GameObject indicatorGO = Instantiate(indicatorPrefab, Vector3.zero, Quaternion.identity, layoutGO.transform);
			indicatorGO.name = "Turn Indicator";
			IndicatorUI indicator = indicatorGO.GetComponent<IndicatorUI>();
			indicator.turnBorder.color = Color.gray;
			if (agent.isNPC==true) { indicator.agentBorder.color = Color.red; }
			else if (agent.isNPC==false) { indicator.agentBorder.color = Color.green; }
			indicators.Add(indicator);
		}	
	}

	public void ActivateTurnIndicator(int turnNum)
	{
		index = turnNum;
		textActive = charactersInTurnOrder[turnNum].isNPC==false;
		for (int i = 0; i<indicators.Count; i++)
		{
			if (indicators[i].isActiveAndEnabled)
			{
				if (i == turnNum) { indicators[i].turnBorder.color = Color.cyan; }
				else { indicators[i].turnBorder.color = Color.gray; }
			}
		}
	}

	public void DeActivateEndTurnButton()
	{
		endTurnIcon.SetActive(false);
	}


}
