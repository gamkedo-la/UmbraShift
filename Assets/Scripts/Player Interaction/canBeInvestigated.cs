using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Interactable))]
public class canBeInvestigated : MonoBehaviour
{

    //Interacts with BaseCharacterClass to allow player to invetigate objects

    public Chatbox chatbox;

    public int BasePercentageChanceToInvestigate = 95;

    public void Investigated()
    {
        Debug.Log("A clue!");
        //TODO: affect chatbox main text and/or options here.
    }

    public void FailedToFindAnything()
    {
        Debug.Log("Nothing of interest here...");
        //TODO: affect chatbox main text and/or options here.
    }
}
