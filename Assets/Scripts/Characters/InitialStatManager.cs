﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitialStatManager : MonoBehaviour
{
    public TMP_InputField inputField;

    public string playerName;

    public Sprite[] playerPortraitOptions;

    public Image playerPortrait;

    public List<int> InitialStats = new List<int>();

    public int currentElementID = 0;

    public int currentPortraitElementID = 0;

    public int maxStatTotal = 20;

    public TMP_Text errorMessage;

    public TMP_Text statTotalNumber;

    private ChangeScene sceneChangeButton;

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
        InitialStats[currentElementID] = baseStat;
        Debug.Log(InitialStats[currentElementID]);
        CheckMaxStatCap();
    }

    void Awake()
    {
        errorMessage.enabled = false;
        playerPortrait.sprite = playerPortraitOptions[currentPortraitElementID];
        sceneChangeButton = FindObjectOfType<ChangeScene>();
    }

    void Update()
    {
        playerPortrait.sprite = playerPortraitOptions[currentPortraitElementID];
    }

    public void NextPortrait()
    {
        if(currentPortraitElementID < playerPortraitOptions.Length-1)
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
        if(currentPortraitElementID >= 1)
        {
            currentPortraitElementID -= 1;
        }
        else
        {
            currentPortraitElementID = playerPortraitOptions.Length-1;
        }
    }

    void CheckMaxStatCap()
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
    }
}
