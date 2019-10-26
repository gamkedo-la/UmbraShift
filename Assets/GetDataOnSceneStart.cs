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

		if (agentStats && agentStats.isNPC==false && playerCharacterData) 
		{
			agentStats.Strength.WriteBaseValue(stat[CharStat.Str]);
			agentStats.Dexterity.WriteBaseValue(stat[CharStat.Dex]);
			agentStats.Intellect.WriteBaseValue(stat[CharStat.Int]);
			//agentStats.Hacking.WriteBaseValue(stat[CharStat.Hack]);
			agentStats.Shooting.WriteBaseValue(stat[CharStat.Shoot]);
			//agentStats.Investigation.WriteBaseValue(stat[CharStat.Investigate]);
			agentStats.Medicine.WriteBaseValue(stat[CharStat.Medicine]);
			//agentStats.FastTalking.WriteBaseValue(stat[CharStat.FastTalking]);
			agentStats.CalculateDerivedValuesFromBaseStats();
		}

		if (agentStats && agentStats.isNPC == true)
		{
			RandomizeInitialStat(agentStats.Strength);
			RandomizeInitialStat(agentStats.Dexterity);
			RandomizeInitialStat(agentStats.Intellect);
			//RandomizeInitialStat(agentStats.Hacking);
			RandomizeInitialStat(agentStats.Shooting);
			//RandomizeInitialStat(agentStats.Investigation);
			RandomizeInitialStat(agentStats.Medicine);
			//RandomizeInitialStat(agentStats.FastTalking);
			agentStats.CalculateDerivedValuesFromBaseStats();
		}
	}

	private void RandomizeInitialStat(Stat stat)
	{
		if (stat.GetValue() == 0) { stat.WriteBaseValue(Random.Range(1, 5)); }
	}
	

}
