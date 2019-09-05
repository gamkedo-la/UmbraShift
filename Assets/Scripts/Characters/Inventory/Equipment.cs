using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipmentSlot;

    private Inventory inventory;

    public override void Use()
    {
        base.Use();

    }
}

public enum EquipmentSlot { primaryWeapon, secondaryWeapon}
