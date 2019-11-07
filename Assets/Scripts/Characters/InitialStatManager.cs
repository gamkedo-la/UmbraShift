using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitialStatManager : MonoBehaviour
{
	public string playerName;
	public int numberOfStats = 5;
	public int maxStatTotal = 13;
	public Sprite[] playerPortraitOptions;
	public Image playerPortrait;

	public TMP_InputField inputField;
	public TMP_Text errorMessage;
	public TMP_Text hintMessage;
	public TMP_Text statTotalNumber;
	public TMP_Text statMaxNumber;
	private string errMessage = "";
	public Button characterComplete;

	public List<int> initialStats = new List<int>() { 0, 0, 0, 0, 0 };
	public int currentElementID = 0;
	public int currentPortraitElementID = 0;
	private ChangeScene sceneChangeButton;

	private bool hintTimerIsOn = false;
	private float hintTimer = 0f;
	private float maxHintTimerValue = 4f;

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
		SetHelpText(currentElementID);
	}

	void Awake()
	{
		initialStats = new List<int>() { 0, 0, 0, 0, 0 };
		errorMessage.enabled = false;
		playerPortrait.sprite = playerPortraitOptions[currentPortraitElementID];
		sceneChangeButton = FindObjectOfType<ChangeScene>();
	}

	
	private void Start()
	{
		characterComplete.gameObject.SetActive(true);
		characterComplete.enabled = true;
		characterComplete.interactable = false;
		int difficulty = (int)PlayerPrefs.GetFloat("Difficulty");
		if (difficulty==0) { difficulty = 3; }
		maxStatTotal = 15 - (difficulty / 2);
		statMaxNumber.text = "/" + maxStatTotal.ToString();
	}

	void Update()
	{
		if (hintTimerIsOn && hintTimer > 0f) { hintTimer = hintTimer - Time.deltaTime; }
		if (hintTimerIsOn && hintTimer <= 0f) { hintTimerIsOn = false; ClearHintMsg(); }

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
		if (!playerCharacterData) 
		{
			GameObject playerCharacterDataGO = new GameObject();
			playerCharacterDataGO.name = "PlayerCharacterData";
			playerCharacterData = playerCharacterDataGO.AddComponent<PlayerCharacterData>();
			playerCharacterData.WriteInitialStatsToCharacterData(initialStats, playerPortrait, playerName);
			LoadingCanvas.ShowLoadingCanvas();
			sceneChangeButton.LoadScene();
		}
		else if (playerCharacterData)
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
		if (playerCharacterData) 
		{
			playerCharacterData.ClearData();
		}
		playerName = "";
		inputField.SetTextWithoutNotify("[Name]");
		initialStats.Clear();
		for (int i = 0; i < numberOfStats; i++) { initialStats.Add(0); }
		errMessage = CharacterCreationErrMsg();
		int statTotal = 0;
		statTotalNumber.text = statTotal.ToString();
	}

	private void SetHelpText(int skillNum)
	{
		string txt = "";
		if (skillNum==0) { txt = "Body is a measure of your toughness and resistance to physical trauma."; }
		if (skillNum == 1) { txt = "Speed gives you additional movement during stressful situations."; }
		if (skillNum == 2) { txt = "Quick wits allow you to make better use of cover when shot at."; }
		if (skillNum == 3) { txt = "Knowledge of human anatomy is useful for targeting vital organs."; }
		if (skillNum == 4) { txt = "Improved firearms accuracy helps you hit the things you shoot at."; }
		hintMessage.text = txt;
		hintTimerIsOn = true;
		hintTimer = maxHintTimerValue;
	}

	private void ClearHintMsg()
	{
		hintMessage.text = "";
	}

	
}
