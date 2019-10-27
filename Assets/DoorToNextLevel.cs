using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoorToNextLevel : MonoBehaviour, IInteractable
{
	[SerializeField] private string levelToGo = "MainMenu";

    private Animator animator;

    public Item ObjectRequiredToOpen;
	public InteractionScreen screenIfNoReqObj;
	public float delay = 0f;




	public void Interact()
	{
        //TODO: check to see if you have a keycard
        //TODO: fade to black
        //TODO: advance to next level
        //TODO: if no keycard, use chatbox to say "it's locked" or something
        //TODO: if not keycard, give option to hack the door?


        if (ObjectRequiredToOpen && Inventory.instance.HasItem(ObjectRequiredToOpen))
        {
			if (GetComponentInChildren<DoorSimpleController>()) { GetComponentInChildren<DoorSimpleController>().OpenDoor(); }
			delay = 2f;
			StartCoroutine(MoveToNextLevel());
        }
		else if (ObjectRequiredToOpen && !Inventory.instance.HasItem(ObjectRequiredToOpen))
		{
			Chatbox chatbox = FindObjectOfType<Chatbox>();
			if (chatbox && screenIfNoReqObj)
			{
				chatbox.Open(screenIfNoReqObj);
			}
		}
		else if (!ObjectRequiredToOpen)
		{
            if (GetComponentInChildren<DoorSimpleController>()) { GetComponentInChildren<DoorSimpleController>().OpenDoor(); }
            delay = 2f;
            StartCoroutine(MoveToNextLevel());
		}


      
	}

    IEnumerator MoveToNextLevel()
    {
		LoadingCanvas.ShowLoadingCanvas();
		yield return new WaitForSeconds(delay);
      
           SceneManager.LoadScene(levelToGo);
    }
}
