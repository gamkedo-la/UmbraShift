using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour
{

    //Interacts with BaseCharacterClass to allow player to hack objects, enemies etc. 

    public int BasePercentageChanceToHack = 95;

    public void BeenHacked()
    {
        Debug.Log("Hack the Planet!");
    }

    public void NotHacked()
    {
        Debug.Log("Access Denied Nerd");
    }
}
