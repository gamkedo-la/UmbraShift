using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialStatManager : MonoBehaviour
{
  

    public List<int> InitialStats = new List<int>();

    public int currentElementID = 0;

    public void SetCurrentElementID(int elementID)
    {
        currentElementID = elementID;
    }

    public void SetBaseStat(int baseStat)
    {
        InitialStats[currentElementID] = baseStat;
    }
}
