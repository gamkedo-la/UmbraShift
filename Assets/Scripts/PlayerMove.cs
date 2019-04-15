using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove instance;
    public Transform target;
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
    }
}
