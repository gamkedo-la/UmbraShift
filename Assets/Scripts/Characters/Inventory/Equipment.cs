using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipmentSlot;

    private Inventory inventory;

    public override void Use()
    {
        base.Use();
        //inventory = equipmentManager.inventory;        
            equipmentManager.Equip(this);              
    }
}

public enum EquipmentSlot { primaryWeapon, secondaryWeapon}
