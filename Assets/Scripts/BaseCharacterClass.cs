using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterClass : MonoBehaviour
{
    public string characterName = "Unknown";

    public Sprite avatar;
    // base character stats
    public int strength = 5;
    public int dexterity = 5;
    public int intelligence = 5;
    public int constitution = 5;
    public int level = 0;
    public int maxHealth;

    // skills
    public int shooting = 0;
    public int hacking = 0;

    private int maxAPRefill = 5;
    private int currentAP;

    public void ActionPointRefill()
    {
        int apRefreshed = 2;
        apRefreshed += Mathf.FloorToInt(dexterity / 2);
        if (apRefreshed > maxAPRefill)
        {
            apRefreshed = maxAPRefill;
        }
        currentAP = apRefreshed;
    }
    
    public bool AttemptToSpend(int cost, bool spendIfWeCan)
    {
        if (cost <= currentAP)
        {
            if (spendIfWeCan)
            {
                currentAP -= cost;
                Debug.Log($"Spent {cost} and have {currentAP} remaining.");
            }
            return true;
        }
        Debug.Log("Couldn't afford so didn't remove cost");
        return false;
    }

}
