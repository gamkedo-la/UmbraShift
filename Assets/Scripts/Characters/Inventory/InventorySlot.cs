﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;
    public TextMeshProUGUI itemName;

    [SerializeField]
    private Button thisItemButton;

    [SerializeField]
    private TextMeshProUGUI numberofItemsInSlotDisplay;

    [SerializeField]
    private TextMeshProUGUI itemDescription;

    public int numberOfItemsInStack = 0;

    private Inventory inventory;

    public Item item;

    

    [SerializeField]
    private GameObject player;

    private void Awake()
    {
        //itemName.text = "Empty";
        //numberofItemsInSlotDisplay.text = numberOfItemsInStack.ToString();
        inventory = player.GetComponent<Inventory>();


        int index = inventory.items.IndexOf(item);
    }

   /* private void Update()
    {
        if(item == null)
        {
            numberofItemsInSlotDisplay.enabled = false;
        }
    }*/

    void OnMouseEnter()
    {
        Debug.Log("on mouse enter inventory slot");
        if(item != null)
        {
            itemDescription.text = item.itemDescription;
        }
    }

    public void OnMouseExit()
    {
        itemDescription.text = "";
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
        //itemName.text = item.name;
    }

    public void AddToNumberInSlot(Item newItem)
    {
        item = newItem;
        int index = inventory.items.IndexOf(newItem);
        numberOfItemsInStack = inventory.itemsInSlot[index];
        numberofItemsInSlotDisplay.enabled = true;

        if(numberOfItemsInStack <= 1)
        {
            numberofItemsInSlotDisplay.text = "";
        }
        else
        {
            numberofItemsInSlotDisplay.text = numberOfItemsInStack.ToString();
        }
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        //itemName.text = "Empty";
        numberOfItemsInStack = 0;
        //numberofItemsInSlotDisplay.text = numberOfItemsInStack.ToString();
        //numberofItemsInSlotDisplay.enabled = false;
    }

    public void OnRemoveButton()
    {
        inventory.Remove(item);
    }

    public void UseItem()
    {
        if(item != null)
        {
            item.Use();
        }
    }
}
