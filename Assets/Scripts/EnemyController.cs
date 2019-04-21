using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    public Transform patrolPointOne;
    public Transform patrolPointTwo;
    public int pauseTime = 3;
    public Text noticeIndicator;
    public float visualRange = 20.0f;

    public float FOV = 90.0f;
    public bool moving = false;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        target = patrolPointOne;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
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
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    moving = false;
                    if (target == patrolPointOne)
                    {
                        target = patrolPointTwo;
                    }
                    else
                    {
                        target = patrolPointOne;
                    }
                    StartCoroutine(PauseForTime());
                }
            }
        }
        Vector3 rayFromMeToPlayer = PlayerController.instance.transform.position - transform.position;
        float degreesNeededToFacePlayer = Quaternion.Angle(transform.rotation,
        Quaternion.LookRotation(rayFromMeToPlayer));
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(FOV / 2, Vector3.up) * transform.forward * 5.0f, Color.red, 0, true);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(FOV / -2, Vector3.up) * transform.forward * 5.0f, Color.red, 0, true);
        if (degreesNeededToFacePlayer < FOV / 2)
        {

            // add distance check before raycast
            RaycastHit rhinfo;
            if (Physics.Raycast(transform.position, rayFromMeToPlayer, out rhinfo, visualRange, 1 << LayerMask.NameToLayer("Player")))
            {
                //Debug.Log(rhinfo.collider.name);
                noticeIndicator.text = rhinfo.collider.name;
            }
        }
        else
        {
            noticeIndicator.text = "?";
        }
    }

    IEnumerator PauseForTime()
    {
        yield return new WaitForSeconds(pauseTime);
        moving = true;
    }
}
