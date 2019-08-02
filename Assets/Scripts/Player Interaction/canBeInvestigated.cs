using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Interactable))]
public class canBeInvestigated : MonoBehaviour
{

    //Interacts with BaseCharacterClass to allow player to invetigate objects

   [SerializeField]
    private Chatbox chatbox;

    [SerializeField]
    private InteractionScreen passScreen;
    [SerializeField]
    private InteractionScreen failScreen;

    public int BasePercentageChanceToInvestigate = 95;

    void Awake()
    {
        chatbox = FindObjectOfType<Chatbox>();
    }

    public void Investigated()
    {
        Debug.Log("A clue!");
        //TODO: affect chatbox main text and/or options here.
        chatbox.Open(passScreen);
    }

    public void FailedToFindAnything()
    {
        Debug.Log("Nothing of interest here...");
        //TODO: affect chatbox main text and/or options here.
        chatbox.Open(failScreen);
    }
}
