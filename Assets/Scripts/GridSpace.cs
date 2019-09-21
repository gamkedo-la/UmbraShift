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
	public static float gridSize = .6f;

	void Start()
    {
		UpdateWorldCamera();
		RectTransform rectTran = GetComponent<RectTransform>();
		rectTran.sizeDelta *= gridSize;
		squareImage = GetComponentInChildren<Image>();
		//squareImage.transform.localScale = new Vector3(
		//									squareImage.transform.localScale.x * gridSize,
		//									squareImage.transform.localScale.y * gridSize,
		//									squareImage.transform.localScale.z);
		string[] validLayerNames = new string[] { "UI", "Floor" };
		LayerMask validLayers = LayerMask.GetMask(validLayerNames);
		Update();
		
    }

	void UpdateWorldCamera()
	{
		cam = Camera.main;
		Canvas canvas = GetComponent<Canvas>();
		if (canvas && cam) { canvas.worldCamera = cam; }
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
		float x = coord.x / gridSize;
		x = Mathf.Round(x);
		x = x * gridSize;
		float y = coord.y;
		float z = coord.z / gridSize;
		z = Mathf.Round(z);
		z = z * gridSize;

		//float gridSize = 1f;
		//float x = Mathf.RoundToInt(coord.x);
		//if ((x % gridSize) != 0) { x = x + 1; }
		//float y = coord.y;
		//float z = Mathf.RoundToInt(coord.z);
		//if ((z % gridSize) != 0) { z = z + 1; }
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
