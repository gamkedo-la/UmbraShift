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
	[SerializeField] bool showVirtualGrid = false;
	private Vector3 TopLeft;
	private Vector3 BottomRight;

	void Start()
    {
		UpdateWorldCamera();
		RectTransform rectTran = GetComponent<RectTransform>();
		rectTran.sizeDelta *= gridSize;
		squareImage = GetComponentInChildren<Image>();

		MeshRenderer rend = transform.parent.GetComponent<MeshRenderer>();
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
		if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.PageUp))
		{ 
			showVirtualGrid = !showVirtualGrid; 
		}
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
		gridCoord.y += 0.02f;
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


	public void OnDrawGizmos()
	{
		if (showVirtualGrid)
		{
			MeshRenderer mesh = transform.parent.GetComponent<MeshRenderer>();
			Vector3 ext = mesh.bounds.extents;
			Vector3 pos = mesh.transform.position;
			pos.x = pos.x - (gridSize * 1.16f);
			pos.z = pos.z - (gridSize * 1.84f);
			Vector3 TopLeft = new Vector3(pos.x - ext.x, pos.y + ext.y, pos.z + ext.z);
			Vector3 TopRight = new Vector3(pos.x + ext.x, pos.y + ext.y, pos.z + ext.z);
			Vector3 BottomLeft = new Vector3(pos.x - ext.x, pos.y + ext.y, pos.z - ext.z);
			Vector3 BottomRight = new Vector3(pos.x + ext.x, pos.y + ext.y, pos.z - ext.z);
			Gizmos.color = Color.blue;
			int columns = (int)(Vector3.Distance(TopLeft, TopRight) / gridSize);
			int rows = (int)(Vector3.Distance(TopLeft, BottomLeft) / gridSize);
			Vector3 start = TopLeft;
			Vector3 end = TopRight;
			for (int i = 0; i < rows; i++)
			{
				Gizmos.DrawLine(start, end);
				start.z = start.z - gridSize;
				end.z = end.z - gridSize;
			}
			start = TopLeft;
			end = BottomLeft;
			for (int j = 0; j < columns; j++)
			{
				Gizmos.DrawLine(start, end);
				start.x = start.x + gridSize;
				end.x = end.x + gridSize;
			}

		}
	}

}
