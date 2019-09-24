using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDataOnSceneStart : MonoBehaviour
{
	PlayerCharacterData playerCharacterData;
	AgentStats agentStats;
	Dictionary<CharStat, int> stat;
    
	private void Start()
    {
		agentStats = GetComponent<AgentStats>();
		playerCharacterData = FindObjectOfType<PlayerCharacterData>();
		if (playerCharacterData) { stat = playerCharacterData.ReadStatsFromCharacterData(); }

		if (agentStats && playerCharacterData) 
		{
			agentStats.Strength.WriteBaseValue(stat[CharStat.Str]);
			agentStats.Dexterity.WriteBaseValue(stat[CharStat.Dex]);
			agentStats.Intellect.WriteBaseValue(stat[CharStat.Int]);
			agentStats.Hacking.WriteBaseValue(stat[CharStat.Hack]);
			agentStats.Shooting.WriteBaseValue(stat[CharStat.Shoot]);
			agentStats.Investigation.WriteBaseValue(stat[CharStat.Investigate]);
			agentStats.Medicine.WriteBaseValue(stat[CharStat.Medicine]);
			agentStats.FastTalking.WriteBaseValue(stat[CharStat.FastTalking]);
			agentStats.CalculateDerivedValuesFromBaseStats();
		}
    }

}
