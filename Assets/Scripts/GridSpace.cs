using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridSpace : MonoBehaviour
{
	public delegate void SquareSelectEventHandler(Vector3 pos, RaycastHit squareInfo);
	public event SquareSelectEventHandler SquareSelected;

	private Camera cam;
	private LayerMask layerMask;
	private const float MAX_DISTANCE = 200f;
	private Image squareImage;
	private RaycastHit _hitInfo;
	static private bool showingGridSquare = false;

	void Start()
    {
		cam = Camera.main;
		Canvas canvas = GetComponent<Canvas>();
		if (canvas && cam) { canvas.worldCamera = cam; }
		squareImage = GetComponentInChildren<Image>();
		string[] validLayerNames = new string[] { "UI", "Floor" };
		LayerMask validLayers = LayerMask.GetMask(validLayerNames);
		Update();
    }
	

	void Update()
	{
		if (showingGridSquare && CheckForValidGridSpace("Floor"))
		{
			squareImage.enabled = true;
			Vector3 gridCoord = GetGridCoord(_hitInfo.point);
			MoveSquareSpaceTo(gridCoord);
			ResolveMouseClicks(_hitInfo);
		}
		else { squareImage.enabled = false; }
    }


	private bool CheckForValidGridSpace(string layer)
	{
		layerMask = LayerMask.NameToLayer(layer);
		LayerMask layerMaskUI = LayerMask.GetMask("UI", layer);
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		return Physics.Raycast(ray, out _hitInfo, MAX_DISTANCE, layerMaskUI);
	}

	static public void ShowGridSquare (bool status)
	{
			showingGridSquare = status;
	}

	private void MoveSquareSpaceTo(Vector3 gridCoord)
	{
		gridCoord.y += 0.2f;
		this.gameObject.transform.position = gridCoord;
	}

	static public Vector3 GetGridCoord (Vector3 coord)
	{
		float x = Mathf.RoundToInt(coord.x);
		float gridSize = 1f;
		if ((x % gridSize) != 0) { x = x + 1; }
		float y = coord.y;
		float z = Mathf.RoundToInt(coord.z);
		if ((z % gridSize) != 0) { z = z + 1; }
		return new Vector3(x, y, z);
	}

	private void ResolveMouseClicks(RaycastHit squareInfo)
	{
		if (Input.GetMouseButtonDown(0))
		{
			OnSquareSelected(squareInfo); 
		}
	}


	protected virtual void OnSquareSelected(RaycastHit squareInfo)
	{
		if (SquareSelected != null)
		{
			SquareSelected(this.gameObject.transform.position, squareInfo);
		}
	}


}
