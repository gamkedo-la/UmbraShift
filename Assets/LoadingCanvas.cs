using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
	GameObject controller;

	public void Start()
	{
		controller = transform.Find("Controller").gameObject;
		ActivateController(false);
	}
	
	public static void ShowLoadingCanvas()
	{
		FindObjectOfType<LoadingCanvas>().ActivateController(true);
	}

	private void ActivateController(bool toggle)
	{
		controller.SetActive(toggle);
	}

}
