using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionBarController : MonoBehaviour
{
	[SerializeField] private Image ActionBarPortrait;
	[SerializeField] private Image ActionBarPortraitFrame;
	[SerializeField] private Button mainButton;
	[SerializeField] private Text activeCharacterName;
	[SerializeField] private GameObject[] skillIcons;
	private Vector3[] skillIconLocations;
	private GameManager turnManager;
	private bool actionBarHidden=false;


	private void Start()
    {
		skillIconLocations = new Vector3[skillIcons.Length];
		turnManager = FindObjectOfType<GameManager>();
		SetSkillIcons();
		HideActionBar();
    }
	
    private void Update()
    {
		if (ActionBarPortrait && turnManager && turnManager.ActiveCharacter && turnManager.ActiveCharacter.PortraitImage) 
		{ 
			ActionBarPortrait.sprite = turnManager.ActiveCharacter.PortraitImage;			
		}

		if (activeCharacterName && turnManager && turnManager.ActiveCharacter)
		{
			 activeCharacterName.text = turnManager.ActiveCharacter.CharacterName;
		}
	}

	public void ButtonClick()
	{
		Debug.Log("Button pressed.");
		if (actionBarHidden==true) { RevealActionBar(); }
		else if (actionBarHidden == false) { HideActionBar(); }
	}

	private void SetSkillIcons()
	{
		for (int i = 0; i < skillIcons.Length; i++)
		{
			skillIconLocations[i] = skillIcons[i].transform.position;
		}
	}

	private void RevealActionBar()
	{
		actionBarHidden = false;
		for (int i = 0; i < skillIcons.Length; i++)
		{
			//skillIcons[i].transform.position = skillIconLocations[i];
			skillIcons[i].SetActive(true);
		}
	}

	private void HideActionBar()
	{
		actionBarHidden = true;
		foreach (GameObject skillIcon in skillIcons) 
		{
			skillIcon.SetActive(false);
		}
	}

}
