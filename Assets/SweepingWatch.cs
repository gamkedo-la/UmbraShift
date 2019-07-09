using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepingWatch : MonoBehaviour
{
	public float maxArc = 90f;
	float speed;
	float rotationAmount = 0f;
	Vector3 defaultAngle;
	Vector3 sweepingAngle;

	private void Start()
	{
		speed = Random.Range(0.2f, 0.5f);
		defaultAngle = transform.forward;
		sweepingAngle = transform.forward;
	}

	void Update()
    {
		rotationAmount = Mathf.Sin(Time.time * speed) * maxArc;
		sweepingAngle = Quaternion.AngleAxis(rotationAmount, transform.up) * defaultAngle;
		transform.localRotation = Quaternion.LookRotation(sweepingAngle);
	}
}
