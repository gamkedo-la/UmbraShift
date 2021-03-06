﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int spaces = 8;

    public List<Item> items = new List<Item>();
    public List<int> itemsInSlot = new List<int>();

    private EquipmentManager equipmentManager;

    public static Inventory instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }


        equipmentManager = GetComponent<EquipmentManager>();
    }

    public bool Add(Item item)
    {
        if(item!=null&&item != item.isDefaultItem)
        {
            if(items.Count >= spaces)
            {
                Debug.Log("Inventory full");
                return false;
            }
            FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.playerPickUpObject);
            if (item.stackable == false)
            {
                items.Add(item);
                itemsInSlot.Add(1);
            }
            if(HasItem(item) == false && item.stackable == true)
            {
                items.Add(item);
                itemsInSlot.Add(0);
            }

            item.equipmentManager = this.equipmentManager;

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
            
        }
        
        return true;
    }

    public void AddToStack(Item item, int amount)
    {
        int index = items.IndexOf(item);
        Debug.Log(item.name + " is in slot " + index);

        if((itemsInSlot[index] + amount) >= item.maxItemsInSlot)
        {
            itemsInSlot[index] = item.maxItemsInSlot;
        }
        else
        {
            itemsInSlot[index] += amount;
        }
        Debug.Log("There are " + itemsInSlot[index].ToString() + " in slot " + index);
        onItemChangedCallback.Invoke();
    }

    public bool HasItem(Item item)
    {
		//return items.Exists(i => i == item);		//I removed this because I've run into errors and I don't
													//know what this line of code does. I've added the below loop instead.
		bool hasItem = false;
		foreach (Item itemInList in items)
		{
			if (itemInList.Equals(item)) { hasItem = true; }
		}
		return hasItem;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        if(onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
