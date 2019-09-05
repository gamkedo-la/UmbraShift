using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public EquipmentSlotDisplay[] equipmentSlotDisplays;

    private void Start()
    {
        equipmentSlotDisplays = GetComponentsInChildren<EquipmentSlotDisplay>();
    }
}
