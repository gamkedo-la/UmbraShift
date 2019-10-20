using UnityEngine;

public enum ItemType { General, Consumable, Pistol, Rifle, Melee }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
	public ItemType itemType = ItemType.General;
	public bool isDefaultItem = false;

    public bool stackable = false;

    public int maxItemsInSlot = 0;

    public string itemDescription = "Item description here.";

    public EquipmentManager equipmentManager;

    public virtual void Use()
    {
        Debug.Log("Using" + name);
        if (this.itemType == ItemType.Consumable) {
            equipmentManager.inventory.Remove(this);
        }
        
    }    

}
