using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBePickedUp : MonoBehaviour
{

    public Item item;

    private BaseCharacterClass baseCharacter;

    private Inventory inventory;
    // Start is called before the first frame update
    void Awake()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.Add(item);
        }
    }
}
