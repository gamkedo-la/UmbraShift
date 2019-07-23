using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
	private Transform cameraHome;
	private Vector3 cameraFocus;
	private const float DIST_THRESHOLD = 0.1f;
	[SerializeField] private float panSpeed = 5f;
	[SerializeField] private float maxDistance = 5f;
	
    public void UpdateCameraHome(GameObject character)
	{
		cameraHome = character.transform;
	}

    // Update is called once per frame
    void Update()
    {
		Vector3 vectorToHome = cameraHome.position - cameraFocus;
		vectorToHome = LimitCamToMaxDistance(vectorToHome);
		if (Vector3.Distance(cameraHome.position, cameraFocus) > DIST_THRESHOLD) 
		{
			
			Vector3.ClampMagnitude(vectorToHome, panSpeed);
		}
    }

	Vector3 LimitCamToMaxDistance(Vector3 vector)
	{
		if (Vector3.Magnitude(vector) > maxDistance)
		{
			return vector.normalized * maxDistance;
		}
		else return vector;
	}
}
