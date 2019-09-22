using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseCharacterClass : MonoBehaviour
{
    public string characterName = "Unknown";

    public bool playerCharacter = false;

    public Sprite avatar;
   
    public int level = 0;
    public int currentHealth;
    public int maxHealth = 10;
    public bool isAlive = true;
    public int BasePercentageChanceToHit = 90;

    // base character stats

    public Stat strength;
    public Stat dexterity;
    public Stat intellect;

    // skills
       
    public Stat hacking;
    public Stat shooting;
    public Stat investigation;
    public Stat medicine;
    public Stat fastTalking;

    //Sounds
    public string takingDamageSound= "event:/Male_Grunt_2";

    [SerializeField]
    private InitialStatManager initialStatManager;

    private int maxAPRefill = 5;
    public int currentAP;

    public void Awake()
    {
        initialStatManager = FindObjectOfType<InitialStatManager>();
        InitializeStats();
        
        //armor.AddModifier(dexterity.GetValue());

        hacking.AddModifier(intellect.GetValue());
        shooting.AddModifier(dexterity.GetValue());
        investigation.AddModifier(intellect.GetValue());
        medicine.AddModifier(intellect.GetValue());
        fastTalking.AddModifier(intellect.GetValue());
    }

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void InitializeStats()
    {
        if (playerCharacter)
        {
            characterName = initialStatManager.playerName;

            avatar = initialStatManager.playerPortraitOptions[initialStatManager.currentPortraitElementID];
            //Initialize Base Stat Values from Character Creation
            strength.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[0]);
            dexterity.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[1]);
            intellect.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[2]);
            hacking.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[3]);
            shooting.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[4]);
            investigation.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[5]);
            medicine.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[6]);
            fastTalking.GetBaseValueFromCharacterCreation(initialStatManager.InitialStats[7]);
        }
        else return;        
    }

    public void ActionPointRefill()
    {
        int apRefreshed = 2;
        apRefreshed += Mathf.FloorToInt(dexterity.GetValue() / 2);
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
        PercentageChanceToHit += shooting.GetValue();        
        PercentageChanceToHit -= target.dexterity.GetValue();
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
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.maleGruntMI, target.gameObject);
        Debug.Log($"I was shot: {target.name} and now have {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            isAlive = false;
            Debug.Log("urg, I've died.");
            PlayDeathSound();
			Destroy( gameObject );
        }
    }
    /**
     * Play Character or enemy death sound effect. 
     * 
     * Update this method with logic to handle different player or enemy sounds.
     * 
    */  
    private void PlayDeathSound()
    {
      // if (playerCharacter)
        //{
            FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.maleDeathMI,gameObject);
        //}
        //else
       // {
       //     FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.maleDeathMI);
        //}
    }

    public void AttemptHack(Hackable target)
    {
        Debug.Log($"{name} hacking {target.name}");
        int PercentageChanceToHack = target.BasePercentageChanceToHack;
        PercentageChanceToHack += hacking.GetValue();

        if (Random.Range(0, 100) <= PercentageChanceToHack)
        {
            target.BeenHacked();
        }
        else
        {
            target.NotHacked();
        }
    }

    public void AttemptInvestigation(canBeInvestigated target)
    {
        Debug.Log($"{name} invetigating {target.name}");
        int PercentageChanceToHack = target.BasePercentageChanceToInvestigate;
        PercentageChanceToHack += investigation.GetValue();

        if (Random.Range(0, 100) <= PercentageChanceToHack)
        {
            target.Investigated();
        }
        else
        {
            target.FailedToFindAnything();
        }
    }

}
