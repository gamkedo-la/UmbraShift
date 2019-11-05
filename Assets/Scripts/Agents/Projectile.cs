using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private float speed = 5f;
	public float Speed { get { return speed; } }
	[SerializeField] private GameObject ExplosionPrefab;
	public GameObject Explosion { get { return ExplosionPrefab; } }
	private Vector3 direction = Vector3.zero;
	private Targetable target;
	private float destructionTimer;


    void Update()
    {
		if (target)
		{
			transform.LookAt(target.TargetPos);
			transform.position = transform.position + (direction * speed * Time.deltaTime);
		}
		if (destructionTimer > 0f) { destructionTimer = destructionTimer - Time.deltaTime; }
		if (destructionTimer < 0f) { Destroy(this.gameObject); }
	}

	public void SetTimer(float timer)
	{
		destructionTimer = timer;
	}

	public void SetTarget (Targetable _target)
	{
		target = _target;
		direction = (target.TargetPos - transform.position).normalized;
	}
	
		
}
