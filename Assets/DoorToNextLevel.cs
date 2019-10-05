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





	public void Interact()
	{
        //TODO: check to see if you have a keycard
        //TODO: fade to black
        //TODO: advance to next level
        //TODO: if no keycard, use chatbox to say "it's locked" or something
        //TODO: if not keycard, give option to hack the door?


        if (Inventory.instance.HasItem(ObjectRequiredToOpen))
        {
            GetComponentInChildren<DoorSimpleController>().OpenDoor();
            StartCoroutine(MoveToNextLevel());
        }


      
	}

    IEnumerator MoveToNextLevel()
    {
        yield return new WaitForSeconds(2);
      
           SceneManager.LoadScene(levelToGo);
    }
}
