using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Inventory inventory;

    public EquipmentUI equipmentUI;

    Equipment[] currentEquipment;

   public Equipment[] GetCurrentEquipment()
    {
        return currentEquipment;
    }

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private void Start()
    {
        int numberOfSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numberOfSlots];
        inventory = GetComponent<Inventory>();

        for (int i = 0; i < currentEquipment.Length; i++)
        {
            equipmentUI.equipmentSlotDisplays[i].equipmentIcon.enabled = false;
        }
    }

    public bool IsItemEquipped(Equipment item)
    {
        return currentEquipment[(int)item.equipmentSlot] == item;
    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipmentSlot;

        int index = inventory.items.IndexOf(newItem);

        Equipment oldItem = null;

        if(currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        if(onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        currentEquipment[slotIndex] = newItem;

        equipmentUI.equipmentSlotDisplays[slotIndex].equipmentIcon.enabled = true;
        equipmentUI.equipmentSlotDisplays[slotIndex].equipmentIcon.sprite = currentEquipment[slotIndex].icon;
        //equipmentUI.equipmentSlotDisplays[slotIndex].itemName.text = currentEquipment[slotIndex].name;

    }

    public void Unequip(int slotIndex)
    {
        if(currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];

            if(slotIndex == 0 || slotIndex == 1)
            {
                inventory.Add(oldItem);
            }

            currentEquipment[slotIndex] = null;

            if(onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }

            equipmentUI.equipmentSlotDisplays[slotIndex].equipmentIcon.enabled = false;
            equipmentUI.equipmentSlotDisplays[slotIndex].equipmentIcon.sprite = null;
            //equipmentUI.equipmentSlotDisplays[slotIndex].itemName.text = "Empty";
        }
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipAll();
        }
    }

}
