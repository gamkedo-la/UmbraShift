using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitialStatManager : MonoBehaviour
{
	public TMP_InputField inputField;

	public int numberOfStats = 5;

	public string playerName;

	public Sprite[] playerPortraitOptions;

	public Image playerPortrait;

	public List<int> initialStats = new List<int>() { 0, 0, 0, 0, 0 };

	public int currentElementID = 0;

	public int currentPortraitElementID = 0;

	public int maxStatTotal = 13;

	public TMP_Text errorMessage;

	public TMP_Text statTotalNumber;

	public TMP_Text statMaxNumber;

	private ChangeScene sceneChangeButton;

	private string errMessage = "";

	public Button characterComplete;

	/*  ELEMENT ID
	 *  0 Strength
	 *  1 Dex
	 *  2 Int
	 *  3 Medicine
	 *  4 Shoot
	 *  5 Hacking
	 *  6 Investigate
	 *  7 Fast Talking
	*/


	public void NameCharacter()
	{
		playerName = inputField.text;
	}

	public void SetCurrentElementID(int elementID)
	{
		currentElementID = elementID;
	}

	public void SetBaseStat(int baseStat)
	{
		Debug.Log("triggered set base stat");
		initialStats[currentElementID] = baseStat;
		Debug.Log(initialStats[currentElementID]);
	}

	void Awake()
	{
		initialStats = new List<int>() { 0, 0, 0, 0, 0 };
		Debug.Log("Stats in InitialStats list: " + initialStats.Count);
		errorMessage.enabled = false;
		playerPortrait.sprite = playerPortraitOptions[currentPortraitElementID];
		sceneChangeButton = FindObjectOfType<ChangeScene>();
	}

	private void Start()
	{
		characterComplete.gameObject.SetActive(true);
		characterComplete.enabled = true;
		characterComplete.interactable = false;
		statMaxNumber.text = "/" + maxStatTotal.ToString();
	}

	void Update()
	{
		playerPortrait.sprite = playerPortraitOptions[currentPortraitElementID];

		int statTotal = 0;
		int numStats = Mathf.Min(numberOfStats, initialStats.Count);
		for (int i = 0; i < numStats; i++)
		{
			statTotal += initialStats[i];
		}
		statTotalNumber.text = statTotal.ToString();
		errMessage = CharacterCreationErrMsg();
		errorMessage.text = errMessage;
		if (errMessage == "")
		{
			sceneChangeButton.gameObject.SetActive(true);
			characterComplete.interactable = true;
			errorMessage.enabled = false;
		}
		else
		{
			characterComplete.interactable = false;
			errorMessage.enabled = true;
		}

	}

	private string CharacterCreationErrMsg()
	{
		bool someStatsHaveValueOfZero = false;
		int statTotal = 0;
		int numStats = Mathf.Min(numberOfStats, initialStats.Count);
		for (int i = 0; i < numStats; i++)
		{
			statTotal += initialStats[i];
			if (initialStats[i] == 0) { someStatsHaveValueOfZero = true; }
		}
		if (statTotal==0) { someStatsHaveValueOfZero = true; }
		int statsPtsAvail = maxStatTotal - statTotal;
		if (someStatsHaveValueOfZero) { return "All stats must have a value."; }
		if (statTotal < maxStatTotal) { return statsPtsAvail.ToString() + " stat points remaining."; }
		if (statTotal > maxStatTotal) { return "Character above max power level."; }
		if (playerName == "") { return "Please enter your name, stream ID, or professional alias."; }
		return "";
	}

	public void NextPortrait()
	{
		if (currentPortraitElementID < playerPortraitOptions.Length - 1)
		{
			currentPortraitElementID += 1;
		}
		else
		{
			currentPortraitElementID = 0;
		}
	}

	public void PreviousPortrait()
	{
		if (currentPortraitElementID >= 1)
		{
			currentPortraitElementID -= 1;
		}
		else
		{
			currentPortraitElementID = playerPortraitOptions.Length - 1;
		}
	}

	/*void CheckMaxStatCap()
    {
        int statTotal = 0;

        for (int i = 0; i < InitialStats.Count; i++)
        {
            statTotal += InitialStats[i];
        }

        statTotalNumber.text = statTotal.ToString();

        if (statTotal > maxStatTotal)
        {
            sceneChangeButton.gameObject.SetActive(false);
            errorMessage.enabled = true;
        }
        else
        {
            sceneChangeButton.gameObject.SetActive(true);
            errorMessage.enabled = false;
        }
    }*/

	public void CharacterComplete()
	{
		PlayerCharacterData playerCharacterData = FindObjectOfType<PlayerCharacterData>();
		if (!playerCharacterData) { playerCharacterData = new PlayerCharacterData(); }
		if (playerCharacterData)
		{
			playerCharacterData.WriteInitialStatsToCharacterData(initialStats, playerPortrait, playerName);
			LoadingCanvas.ShowLoadingCanvas();
			sceneChangeButton.LoadScene();
		}

	}

	public void ResetCharacter()
	{
		Toggle[] toggles = FindObjectsOfType<Toggle>();
		foreach (Toggle toggle in toggles)
		{
			toggle.interactable = true;
			toggle.isOn = false;
		}
		PlayerCharacterData playerCharacterData = FindObjectOfType<PlayerCharacterData>();
		if (playerCharacterData) { Destroy(playerCharacterData); }
		playerName = "";
		inputField.SetTextWithoutNotify("[Name]");
		initialStats.Clear();
		for (int i = 0; i < numberOfStats; i++) { initialStats.Add(0); }
		errMessage = CharacterCreationErrMsg();
		int statTotal = 0;
		statTotalNumber.text = statTotal.ToString();
	}
}
