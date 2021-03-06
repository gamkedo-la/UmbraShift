﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController3 : MonoBehaviour
{

	private float camSpeed = 10f;
	private float borderSizePercent = 10f;
	private Camera cam;
	public float MinXConstraint = Mathf.Infinity;
	public float MaxXConstraint = Mathf.Infinity;
	public float MinZConstraint = Mathf.Infinity;
	public float MaxZConstraint = Mathf.Infinity;
	private bool returning = false;
	private float counter;
	private Vector3 offset;
	private enum Axis { x,y,z}
	

	private void Start()
	{
		cam = Camera.main;
		offset = transform.position;
	}

	void Update()
	{
        // https://forum.unity.com/threads/detect-when-mouse-leaves-the-screen-area.411252/


#if UNITY_EDITOR
        if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1) return;
#else
    if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) return;
#endif


        if (Input.GetKeyDown(KeyCode.Space)) { CenterOn(GameObject.FindGameObjectWithTag("Player").transform); }
		if (returning == false)
		{
			Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition);
			Vector3 vec = Vector3.zero;
			/*float speed = camSpeed;
			if (ScreenEdge(pos, Axis.x, 0.85f, 0.9f)) { vec = vec - Vector3.right; speed = camSpeed * 0.5f; }
			if (ScreenEdge(pos, Axis.x, 0.9f, 0.95f)) { vec = vec - Vector3.right; speed = camSpeed * 1; }
			if (ScreenEdge(pos, Axis.x, 0.95f, 1f)) { vec = vec - Vector3.right; speed = camSpeed * 2; }
			if (ScreenEdge(pos, Axis.x, 0.1f, 0.15f)) { vec = vec + Vector3.right; speed = camSpeed * 0.5f; }
			if (ScreenEdge(pos, Axis.x, 0.05f, 0.1f)) { vec = vec + Vector3.right; speed = camSpeed * 1; }
			if (ScreenEdge(pos, Axis.x, 0f, 0.05f)) { vec = vec + Vector3.right; speed = camSpeed * 2; }
			if (ScreenEdge(pos, Axis.y, 0.85f, 0.9f)) { vec = vec - Vector3.right; speed = camSpeed * 0.5f; }
			if (ScreenEdge(pos, Axis.y, 0.9f, 0.95f)) { vec = vec - Vector3.forward; speed = camSpeed * 1; }
			if (ScreenEdge(pos, Axis.y, 0.95f, 1f)) { vec = vec - Vector3.forward; speed = camSpeed * 2; }
			if (ScreenEdge(pos, Axis.y, 0.1f, 0.15f)) { vec = vec + Vector3.right; speed = camSpeed * 0.5f; }
			if (ScreenEdge(pos, Axis.y, 0.05f, 0.1f)) { vec = vec + Vector3.forward; speed = camSpeed * 1; }
			if (ScreenEdge(pos, Axis.y, 0f, 0.05f)) { vec = vec + Vector3.forward; speed = camSpeed * 2; }
			vec = vec.normalized * speed * Time.deltaTime;
			cam.transform.localPosition = cam.transform.localPosition + vec;
			*/
			float percent = 0f;
			Vector3 dir = Vector3.zero;
			if (pos.x > 0.85f) 
			{
				float minToBreach = 0.85f;
				float breach = pos.x - minToBreach;
				float maxBreach = 0.15f;
				float breachPercent = breach / maxBreach;
				percent = breachPercent;
				dir = Vector3.right; 
			}
			if (pos.x < 0.15f)
			{
				float maxToBreach = 0.15f;
				float breach = maxToBreach - pos.x;
				float maxBreach = 0.15f;
				float breachPercent = breach / maxBreach;
				percent = breachPercent;
				dir = Vector3.left;
			}
			if (pos.y > 0.85f)
			{
				float minToBreach = 0.85f;
				float breach = pos.y - minToBreach;
				float maxBreach = 0.15f;
				float breachPercent = breach / maxBreach;
				percent = breachPercent;
				dir = Vector3.forward;
			}
			if (pos.y < 0.15f)
			{
				float maxToBreach = 0.15f;
				float breach = maxToBreach - pos.y;
				float maxBreach = 0.15f;
				float breachPercent = breach / maxBreach;
				percent = breachPercent;
				dir = Vector3.back;
			}
			float speed = (camSpeed * 2) * percent;
			
			/*
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			int layerMask = LayerMask.GetMask("UI");
			bool hitUI = Physics.Raycast(ray, 100f, layerMask);
			if (hitUI) { dir = Vector3.zero; speed = 0f; }
			*/

			vec = dir.normalized * speed * Time.deltaTime;
			Vector3 newPos = cam.transform.localPosition - vec;

			if (newPos.x < MinXConstraint) { newPos.x = MinXConstraint; }
			if (newPos.x > MaxXConstraint) { newPos.x = MaxXConstraint; }
			if (newPos.z < MinZConstraint) { newPos.x = MinZConstraint; }
			if (newPos.z > MaxZConstraint) { newPos.x = MaxZConstraint; }
			cam.transform.localPosition = cam.transform.localPosition - vec;
		
		}
	}

	private bool ScreenEdge(Vector2 screenPosition, Axis axis, float startPercent, float endPercent)
	{
		float pos;
		if (axis==Axis.x) { pos = screenPosition.x; }
		else if (axis==Axis.y) { pos = screenPosition.y; }
		else { return false; }

		return pos > startPercent && pos < endPercent;		
	}

	public void CenterOn(Transform character)
	{
		returning = true;
		counter = 0;
		Vector3 start = transform.position;
		Vector3 end = character.transform.position + offset;
		float duration = Vector3.Distance(start, end) / 40f;
		StartCoroutine(MoveCam(start, end, duration));
	}

	public IEnumerator MoveCam(Vector3 start, Vector3 end, float duration)
	{
		float counter = 0;
		while (counter<duration)
		{
			counter = counter + Time.deltaTime;
			float percentTime = counter / duration;
			transform.position = Vector3.Lerp(start, end, percentTime);
			yield return null;
		}
		transform.position = end;
		returning = false;
	}
}
