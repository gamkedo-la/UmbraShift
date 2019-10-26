using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollingCredits : MonoBehaviour
{

	private float scrollingSpeed = 10f;
	private float baseScrollingSpeed = 10f;
	private float scrollRateIncrease = 0.5f;
	private bool isScrolling = true;
	private int scrollLimit = 1650;
	private int UIfactor = 10;
	private Vector3 startPos;

	private void Awake()
	{
		startPos = transform.position;
	}
	void Update()
    {
		if (transform.position.y < scrollLimit)
		{
			if (isScrolling) { transform.position = transform.position + (Vector3.up * UIfactor * scrollingSpeed * Time.deltaTime); }
		}
		if (Input.GetMouseButtonDown(0) || Input.anyKeyDown) 
		{ 
			isScrolling = !isScrolling; 
			scrollingSpeed = baseScrollingSpeed; 
			if (transform.position.y >= scrollLimit) 
			{
				transform.position = startPos;
				isScrolling = true;
			} 
		}
    }

	private void OnEnable()
	{
		transform.position = startPos;
		isScrolling = true;
	}
}
