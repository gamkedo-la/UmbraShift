using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Interactable))]
public class Hackable : MonoBehaviour
{

    //Interacts with BaseCharacterClass to allow player to hack objects, enemies etc. 

    public int BasePercentageChanceToHack = 95;

    [SerializeField]
    private Chatbox chatbox;

    [SerializeField]
    private InteractionScreen passScreen;
    [SerializeField]
    private InteractionScreen failScreen;

    void Awake()
    {
        chatbox = FindObjectOfType<Chatbox>();
    }

    public void BeenHacked()
    {
        Debug.Log("Hack the Planet!");
        //TODO: affect chatbox main text and/or options here.
        chatbox.Open(passScreen);
    }

    public void NotHacked()
    {
        Debug.Log("Access Denied Nerd");
        //TODO: affect chatbox main text and/or options here.
        chatbox.Open(failScreen);
    }
}
