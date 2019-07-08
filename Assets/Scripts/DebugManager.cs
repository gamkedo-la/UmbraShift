using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public Text debugText;
    public GameObject debugPanel;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("singleton already existed but attempted re-assignment");
        }
        else
        {
            instance = this;
        }
    }

    public void OverwriteDebugText(string newText)
    {
        debugText.text = newText;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            debugPanel.SetActive(debugPanel.activeSelf == false);
        }
    }
}
