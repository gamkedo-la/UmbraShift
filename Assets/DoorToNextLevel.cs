using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoorToNextLevel : MonoBehaviour, IInteractable
{
	[SerializeField] private string levelToGo = "MainMenu";

	public void Interact()
	{
		//TODO: check to see if you have a keycard
		//TODO: fade to black
		//TODO: advance to next level
		//TODO: if no keycard, use chatbox to say "it's locked" or something
		//TODO: if not keycard, give option to hack the door?
		SceneManager.LoadScene(levelToGo);
	}
}
