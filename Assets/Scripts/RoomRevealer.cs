using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRevealer : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
//            Debug.Log($"{other.name} is tagged as player");
            gameObject.SetActive(false);
        }
    }
}
