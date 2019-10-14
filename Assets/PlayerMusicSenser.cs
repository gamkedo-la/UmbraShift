using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMusicSenser : MonoBehaviour
{

    Transform playerTransform;
    public float senserRadius=5f;
    void Start()
    {
        playerTransform = transform;

        if (!AgentTurnManager.instance.turnManagerActiveInScene)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, senserRadius);
       
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.CompareTag("NPC")){
                UmbraEventManager.instance.ActivateAlarm();
                return;
            }


        }

        if (UmbraEventManager.instance.alertState)
        {
            UmbraEventManager.instance.DeactivateAlarm();
        }
    }
}
