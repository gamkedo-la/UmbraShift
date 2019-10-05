using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController4 : MonoBehaviour
{
	[Header("Max Move Allowance")]
	[SerializeField] float north = 0f;
	[SerializeField] float east = 0f;
	[SerializeField] float south = 0f;
	[SerializeField] float west = 0f;
	private Transform focus;
	private Vector3 origin;
	private Camera cam;
	private Vector3 offset;
	private float camScrollSpeed = 5f;
	private const float SCREEN_BORDER_THRESHOLD = 0.04f;

	private void Start()
	{
		cam = Camera.main;
		focus = GameObject.Find("CameraFocus").transform;
		origin = focus.position;
		offset = transform.position - origin;
	}
	

	public void LateUpdate()
	{
		Vector3 move = Vector3.zero;
		Vector2 mousePos = cam.ScreenToViewportPoint(Input.mousePosition);
		if (mousePos.x > 1-SCREEN_BORDER_THRESHOLD) { move = move + Vector3.right; }
		if (mousePos.x < 0+SCREEN_BORDER_THRESHOLD) { move = move - Vector3.right; }
		if (mousePos.y > 1-SCREEN_BORDER_THRESHOLD) { move = move + Vector3.forward; }
		if (mousePos.y < 0+SCREEN_BORDER_THRESHOLD) { move = move - Vector3.forward; }
		if (move != Vector3.zero) { move = move * camScrollSpeed * Time.deltaTime; }
		Vector3 dest = focus.position + move;
		if (move.x > 0f) { dest.x = Mathf.Clamp(dest.x, origin.x-west, origin.x+east); }
		if (move.x < 0f) { dest.x = Mathf.Clamp(dest.x, origin.x-west, origin.x+east); }
		if (move.z > 0f) { dest.z = Mathf.Clamp(dest.z, origin.z-south, origin.z+north); }
		if (move.z < 0f) { dest.z = Mathf.Clamp(dest.z, origin.z-south, origin.z+north); }
		focus.position = dest;
		transform.position = focus.position + offset;
	}

	

}
