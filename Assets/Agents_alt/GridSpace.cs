using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridSpace : MonoBehaviour
{
	public delegate void SquareSelectEventHandler(Vector3 pos);
	public event SquareSelectEventHandler SquareSelected;

	private Camera cam;
	private LayerMask layerMask;
	private const float MAX_DISTANCE = 200f;
	private Image squareImage;
	private RaycastHit _hitInfo;

	void Start()
    {
		cam = Camera.main;
		squareImage = GetComponentInChildren<Image>();
    }


	void Update()
	{
		if (CheckForValidGridSpace("Floor"))
		{
			ShowGridSquare(true);
			MoveSquareSpaceToMouseCursor();
			ResolveMouseClicks();
		}
		else { ShowGridSquare(false); }
    }

	private bool CheckForValidGridSpace(string layer)
	{
		layerMask = LayerMask.NameToLayer(layer);
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		return Physics.Raycast(ray, out _hitInfo, MAX_DISTANCE, -layerMask);
	}

	private void ShowGridSquare (bool status)
	{
			squareImage.enabled = status;
	}

	private void MoveSquareSpaceToMouseCursor()
	{
		Vector3 spaceHit = GetGridCoord(_hitInfo.point);
		this.gameObject.transform.position = spaceHit;
	}


	public Vector3 GetGridCoord (Vector3 coord)
	{
		float x = Mathf.RoundToInt(coord.x);
		if (x % 2 != 0) { x = x + 1; }
		float y = coord.y;
		float z = Mathf.RoundToInt(coord.z);
		if (z % 2 != 0) { z = z + 1; }
		return new Vector3(x, y, z);
	}

	private void ResolveMouseClicks()
	{
		if (Input.GetMouseButtonDown(0))
		{
			OnSquareSelected();
		}
	}


	protected virtual void OnSquareSelected()
	{
		if (SquareSelected != null)
		{
			SquareSelected(transform.position);
		}
	}


}
