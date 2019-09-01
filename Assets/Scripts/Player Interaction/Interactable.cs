using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Material defaultMat;
    private Renderer currentRenderer;

    // Chatbox trigger code
    private GameObject ChatboxGUI;
    private bool alreadyChatted = false;
    public InteractionScreen NPC_Chat;


    // Start is called before the first frame update
    void Start()
    {
        currentRenderer = GetComponent<Renderer>();
        defaultMat = currentRenderer.sharedMaterial;  // might need to do individual, check later
        ChatboxGUI =  GameObject.Find("Chatbox"); // the GUI
        alreadyChatted = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ObjectDeselected()
    {
//        Debug.Log("Object has become deselected!");
        currentRenderer.material = defaultMat;
    }

    public virtual void ObjectSelected()
    {
//        Debug.Log("Setting highlight material");
        currentRenderer.material = InputManager.instance.highlightedMat;

        //Debug.Log("Testing NPC Chat Trigger...");
        // FIXME: only trigger when not targetting in combat?
        if (!alreadyChatted && ChatboxGUI != null && NPC_Chat != null) {
            Debug.Log("Triggering an NPC Chat!");
            Chatbox Chat = ChatboxGUI.GetComponent<Chatbox>();
            Chat.Open(NPC_Chat);
            alreadyChatted = true;
        }
    }
}
