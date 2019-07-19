using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canBeInvestigated : MonoBehaviour
{

    //Interacts with BaseCharacterClass to allow player to invetigate objects

    public int BasePercentageChanceToInvestigate = 95;

    public void Investigated()
    {
        Debug.Log("A clue!");
    }

    public void FailedToFindAnything()
    {
        Debug.Log("Nothing of interest here...");
    }
}
