using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementTracer : MonoBehaviour
{
	private NavMeshAgent AI;
	private ParticleSystem PS;
	private TrailRenderer tail;
	private Vector3 destinationPoint;
	
    void Start()
    {
		AI = GetComponent<NavMeshAgent>();
		PS = GetComponent<ParticleSystem>();
    }

	public void Reset()
	{
		PS.Stop();
		AI.isStopped = true;
		tail.Clear();
		tail.enabled = false;
		transform.position = transform.parent.position;
	}
	
	public void Go(Vector3 dest)
	{
		PS.Play();
		AI.isStopped = false;
		AI.SetDestination(dest);
		tail.enabled = true;
	}

    
}
