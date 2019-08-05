using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    public static MissionController instance;
    public Dictionary<string, bool> missions = new Dictionary<string, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("something else is already the mission controller");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Text[] initialMissions = GetComponentsInChildren<Text>();

        for (int i = 0; i < initialMissions.Length; i++)
        {
            missions.Add(initialMissions[i].text, false);
        }
    }

    public void MarkMissionDone(string mission)
    {
        missions[mission] = true;
        Text[] initialMissions = GetComponentsInChildren<Text>();
        bool matchFound = false;
        for (int i = 0; i < initialMissions.Length; i++)
        {
            if (initialMissions[i].text == mission)
            {
                initialMissions[i].fontStyle = FontStyle.Italic;
                matchFound = true;
            }
        }

        if (matchFound == false)
        {
            Debug.Log($"Tried to find {mission} but was unable to.");
        }
    }
}
