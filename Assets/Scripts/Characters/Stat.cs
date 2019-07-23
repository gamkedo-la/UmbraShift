using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat  
{

    [SerializeField]
    private int baseValue;

    [SerializeField]
    private List<int> modifiers = new List<int>();

    public int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return finalValue;
    }

    public void  AddModifier(int modifer)
    {
        if(modifer != 0)
        {
            modifiers.Add(modifer);
        }
    }

    public void RemoveModifier(int modifier)
    {
        if(modifier != 0)
        {
            modifiers.Remove(modifier);
        }
    }
}
