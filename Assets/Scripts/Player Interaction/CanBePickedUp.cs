using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBePickedUp : Interactable
{

    public Item item;    

    private Inventory inventory;
 
    public override void ObjectSelected()
    {
        base.ObjectSelected();
        inventory = TurnManager.instance.ActivePlayerController.gameObject.GetComponent<Inventory>();
        inventory.Add(item);
        //  Destroy(gameObject);
    }

   
}
