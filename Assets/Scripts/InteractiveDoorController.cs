using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractiveDoorController : MonoBehaviour,IInteractable

	       
{
    // public int exitToLevel;


    
    public bool doorOpen=false;
    public Item ObjectRequiredToOpen;
	public InteractionScreen ConvIfNoRequiredObj;
    public float delay = 0f;
	private List<Collider> openedColliders = new List<Collider>();

    public void PlayOpeningSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.doorSimpleOpeningWithKeyPad,gameObject);
    }

    public void PlayClosingSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.doorSimpleClosing,gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        AgentMovement player = other.GetComponent<AgentMovement>();


        if (player != null )
            
        {
            OpenDoor();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //AgentMovement player = other.GetComponent<AgentMovement>();


        //if (player != null)
        //{
           
        //}
    }

    private void Update()
    {
        //if (GetComponent<Animator>().GetBool("isOpen")&& Input.GetKeyDown(KeyCode.Space))
        //{
        //    SceneManager.LoadScene(exitToLevel);
        //}
    }

    public void OpenDoor()
    {
        GetComponentInChildren<Animator>().SetBool("isOpen", true);
        doorOpen = true;
		Collider[] doorColliders = GetComponentsInChildren<Collider>();
		openedColliders.Clear();
		foreach (Collider collider in doorColliders)
		{
			if (!collider.isTrigger) 
			{
				openedColliders.Add(collider);
				collider.enabled = false; 
			}
		}
    }

    public void CloseDoor()
    {
        GetComponent<Animator>().SetBool("isOpen", false);
        doorOpen = false;
		foreach (Collider collider in openedColliders)
		{
			collider.enabled = true;
			openedColliders.Remove(collider);
		}
	}

    public void Interact()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        Inventory playerInventory = playerObject.GetComponent<Inventory>();

        if (ObjectRequiredToOpen && playerInventory.HasItem(ObjectRequiredToOpen))
        {
            delay = 2f;
            OpenDoor();
        }
		else if (ObjectRequiredToOpen && !playerInventory.HasItem(ObjectRequiredToOpen))
		{
			Chatbox chatbox = FindObjectOfType<Chatbox>();
			if (ConvIfNoRequiredObj && chatbox)
			{
				chatbox.Open(ConvIfNoRequiredObj);
			}
		}
		else if (!ObjectRequiredToOpen)
		{
			delay = 1f;
			OpenDoor();
		}
        
    }
}
