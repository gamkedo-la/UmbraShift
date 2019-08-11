using System.Collections;
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
        InitialStats[currentElementID] = baseStat;
    }

    void Awake()
    {
        playerPortrait.sprite = playerPortraitOptions[0];
    }
}
