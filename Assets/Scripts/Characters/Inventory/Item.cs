using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;

    public bool stackable = false;

    public int maxItemsInSlot = 0;

    public string itemDescription = "Item description here.";

    public virtual void Use()
    {
        Debug.Log("Using" + name);
        //TODO Access Inventory and remove item
    }    

}
