using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projecile : MonoBehaviour
{
	[SerializeField] float speed = 5f;
	[SerializeField] GameObject ExplosionPrefab;
	GameObject explosionEffect;
	float maxDistance = 50f;
	[SerializeField] float distance = 0f;
	[SerializeField] bool explosionGenerated = false;

	
	void Update()
    {
		distance = distance + (speed * Time.deltaTime);
		Vector3 vec = transform.forward * (speed * Time.deltaTime);
		transform.localPosition = transform.localPosition + vec;
		if (distance >= maxDistance) { Destroy(this.gameObject); }
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Agent") ||
			collision.gameObject.layer == LayerMask.NameToLayer("NPC") ||
			collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			explosionGenerated = true;
			explosionEffect = Instantiate(ExplosionPrefab, collision.contacts[0].point, Quaternion.identity);
			Destroy(explosionEffect, 10f);
			Destroy(this.gameObject,0.1f);
		}
	}		
}
