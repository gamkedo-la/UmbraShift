using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
	private Transform cameraHome;
	[SerializeField] private Transform cameraDest;
	private Vector3 cameraPos;
	private Vector3 cameraOffset;

	private bool cameraActive = false;
	private const float DIST_THRESHOLD = 0.1f;
	[SerializeField] private float panSpeed = 5f;
	[SerializeField] private float maxDistance = 5f;
	public bool CAMERA_ACTIVE { get { return cameraActive; } }

	public void InitialzieCameraHome(GameObject character)
	{
		AssignCameraHome(character);
		cameraOffset = transform.position - character.transform.position;
		cameraActive = true;
	}
	public void AssignCameraHome(GameObject character)
	{
		cameraHome = character.transform;
		cameraDest.position = character.transform.position;
	}

	public void ResetCameraDest()
	{
		cameraDest.position = cameraHome.position;
	}

	public void MoveCameraDest(Vector3 movement)
	{
		if (cameraActive)
		{
			movement = transform.rotation * movement;
			movement.y = 0f;
			movement = movement * panSpeed * Time.deltaTime;
			cameraDest.position = cameraDest.position + movement;
			if (Vector3.Distance(cameraDest.position, cameraHome.position) > maxDistance)
			{
				var vector = cameraDest.position - cameraHome.position;
				vector.y = 0f;
				vector = vector.normalized * maxDistance;
				cameraDest.position = cameraHome.position + vector;
			}
		}
	}

	public void MoveCameraTowardDest()
	{
		if (cameraActive)
		{
			Vector3 vector = cameraDest.position - cameraPos;
			Vector3.ClampMagnitude(vector, panSpeed);
			vector = vector.normalized * (vector.magnitude * Time.deltaTime);
			Vector3.ClampMagnitude(vector, Vector3.Distance(cameraPos, cameraDest.position));
			cameraPos = cameraPos + vector;
		}
	}

    void LateUpdate()
    {
		if (cameraActive)
		{
			if (Vector3.Distance(cameraPos, cameraDest.position) > DIST_THRESHOLD)
			{
				MoveCameraTowardDest();
			}
			transform.position = cameraPos + cameraOffset;
		}
    }

	
}
