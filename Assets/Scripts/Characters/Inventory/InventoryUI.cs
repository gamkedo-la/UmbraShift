using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public Transform itemsParent;

    public GameObject inventoryUI;

    [SerializeField]
    Inventory inventory;

    InventorySlot[] slots;

    public string inventoryMenuInput = "Inventory";

    // Start is called before the first frame update
    void Start()
    {
        
        //inventory = TurnManager.instance.MainCharacterController.gameObject.GetComponent<Inventory>();
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(inventoryMenuInput))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);

        }
    }

    void UpdateUI()
    {
        Debug.Log("Updating UI");
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
