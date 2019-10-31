using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlaceOnNavMesh : MonoBehaviour
{
	void Start()
	{
		NavMeshHit closestHit;

		if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500f, NavMesh.AllAreas))
			gameObject.transform.position = closestHit.position;
		else
			Debug.LogError(gameObject.name + " could not find position on NavMesh!");
	}
}

