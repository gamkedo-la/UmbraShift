using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    //Basic Trait Stats
    public Stat strength;
    public Stat dexterity;
    public Stat constitution;
    public Stat intelligence;
    public Stat perception;
    
    //Skill Stats
    public Stat hacking;
    public Stat shooting;
    public Stat investigation;
    public Stat stealth;

    public void Awake()
    {
        maxHealth += constitution.GetValue();
        armor.AddModifier(dexterity.GetValue());

        hacking.AddModifier(intelligence.GetValue());
        shooting.AddModifier(dexterity.GetValue());
        investigation.AddModifier(perception.GetValue() + intelligence.GetValue());
        stealth.AddModifier(dexterity.GetValue());
    }
    //TODO: Add Functionality for Equipment to modify player stats
}
