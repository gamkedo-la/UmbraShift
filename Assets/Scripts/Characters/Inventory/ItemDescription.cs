using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ItemDescription : MonoBehaviour, IPointerEnterHandler
{  
    public TextMeshProUGUI itemDescription;
    public InventorySlot inventorySlot;
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemDescription == null)
        {
            return;
        }



        if(inventorySlot.item != null)
        {
            itemDescription.text = inventorySlot.item.itemDescription;
        }
        else
        {
           itemDescription.text = "";
        }
    }

   
}
