using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Transform target;
    public float visualRange = 30.0f;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        if (instance != null)
        {
            Debug.Log("singleton already existed but attempted re-assignment");
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 travelDiff = target.position - transform.position;
        travelDiff.y = 0.0f;

        Vector3 moveDiff = agent.velocity;
        moveDiff.y = 0.0f;

        if (travelDiff.magnitude > 0.5 && moveDiff.magnitude > 0.1)
        {
            transform.rotation = Quaternion.LookRotation(moveDiff);
        }
        agent.SetDestination(target.position);

        CheckLineOfSite();
    }

    void CheckLineOfSite()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, visualRange);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "NPC")
            {
                Debug.Log("NPC is in the overlapsphere");
                Vector3 rayFromMeToNPC = hitColliders[i].gameObject.transform.position - transform.position;
                RaycastHit rhinfo;
                if (Physics.Raycast(transform.position, rayFromMeToNPC, out rhinfo, visualRange))
                {
                    //Debug.Log(rhinfo.collider.name);
                    if (rhinfo.collider.tag == "NPC")
                    {
                        hitColliders[i].gameObject.layer = LayerMask.NameToLayer("VisibleNPC");
                    }
                    else
                    {
                        hitColliders[i].gameObject.layer = LayerMask.NameToLayer("HiddenNPC");
                    }
                }
            }
        }
    }
}
