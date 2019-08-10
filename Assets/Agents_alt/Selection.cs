using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selection : MonoBehaviour
{
	Image image;
	GridSpace gridSpace;

    void Start()
    {
		image = GetComponentInChildren<Image>();
		ShowSelectionMarker(false);
		gridSpace = FindObjectOfType<GridSpace>();
		gridSpace.SquareSelected += OnSquareSelected;
    }

	void ShowSelectionMarker(bool status)
	{
		image.enabled = status;
	}
    
    void OnSquareSelected(Vector3 pos)
    {
		ShowSelectionMarker(true);
		this.transform.position = pos;
    }
}
