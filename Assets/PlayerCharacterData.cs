using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharStat { Str=0, Dex=1, Int=2, Hack=3, Shoot=4, Investigate=5, Medicine=6, FastTalking=7}
public class PlayerCharacterData : MonoBehaviour
{
	public Dictionary<CharStat, int> stats = new Dictionary<CharStat, int>();
	public string playerName;
	public Image playerPortrait;
	public int story = 0;

	public void WriteInitialStatsToCharacterData(List<int> initialStats, Image portrait, string name)
	{
		stats[CharStat.Str] = initialStats[0];
		stats[CharStat.Dex] = initialStats[1];
		stats[CharStat.Int] = initialStats[2];
		stats[CharStat.Hack] = initialStats[3];
		stats[CharStat.Shoot] = initialStats[4];
		stats[CharStat.Investigate] = initialStats[5];
		stats[CharStat.Medicine] = initialStats[6];
		stats[CharStat.FastTalking] = initialStats[7];
		playerName = name;
		playerPortrait = portrait;
	}

	public Dictionary<CharStat,int> ReadStatsFromCharacterData()
	{
		return stats;
	}

	public void AdvanceStory()
	{
		story = story + 1;
	}

}
