using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projecile : MonoBehaviour
{
	[SerializeField] float speed = 5f;
	[SerializeField] GameObject ExplosionPrefab;
	float maxDistance = 50f;
	float distance = 0f;

    void Update()
    {
		distance = distance + (speed * Time.deltaTime);
		Vector3 vec = transform.forward * (speed * Time.deltaTime);
		transform.localPosition = transform.localPosition + vec;
		if (distance >= maxDistance) { Destroy(this.gameObject); }
    }

	private void OnCollisionEnter(Collision collision)
	{
		//TODO: work out collisions
	}
}
