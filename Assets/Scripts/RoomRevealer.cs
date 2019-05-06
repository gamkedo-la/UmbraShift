using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRevealer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.tag == "Player")
        {
            Debug.Log($"{other.name} is tagged as player");
            gameObject.SetActive(false);
        }
    }
}
