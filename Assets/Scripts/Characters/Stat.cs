﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat  
{

    [SerializeField]
    private int baseValue=0;

    [SerializeField]
    private List<int> modifiers = new List<int>();

    public int GetBaseValueFromCharacterCreation(int sheetValue)
    {
        baseValue = sheetValue;
        return baseValue;
    }

	public int WriteBaseValue(int _baseValue)
	{
		baseValue = _baseValue;
		return baseValue;
	}

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
