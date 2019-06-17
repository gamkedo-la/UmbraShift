using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public int currentHealth;
    public int maxHealth = 10;
    public bool isAlive = true;
    private int BasePercentageChanceToHit = 95;
    

    // skills
    public int shooting = 0;
    public int hacking = 0;

    private int maxAPRefill = 5;
    private int currentAP;

    public void Start()
    {
        currentHealth = maxHealth;
    }

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

    public void ShootAtTarget(BaseCharacterClass target)
    {
        Debug.Log($"{name} shooting {target.name}");
        int PercentageChanceToHit = BasePercentageChanceToHit;
        PercentageChanceToHit += dexterity + shooting;
        PercentageChanceToHit -= target.dexterity;
        Debug.Log($"chance to hit {PercentageChanceToHit}");
        if (Random.Range(0, 100) <= PercentageChanceToHit)
        {
            target.BeenShot(target);
        }
        else
        {
            Debug.Log("Shot missed!");
        }
        
    }

    public void BeenShot(BaseCharacterClass target)
    {
        currentHealth -= 2;
        Debug.Log($"I was shot: {target.name} and now have {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            isAlive = false;
            Debug.Log("urg, I've died.");
        }
    }

}
