using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSelectionController : MonoBehaviour
{
	private GridSpace gridSpace;
	private Action actionAvailableInSquare;
	public delegate void SelectionEventHandler(Vector3 pos, RaycastHit squareInfo, Action actionAvailableInSquare);
	public delegate void RightClickEventHandler();
	public event SelectionEventHandler selectedSquare;
	public event RightClickEventHandler RightClick;
	void Start()
	{
		gridSpace = FindObjectOfType<GridSpace>();
		gridSpace.SquareSelected += ClickedSquareOnGrid;
		actionAvailableInSquare = Action.None;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			RightClickPressed();
		}
	}

	protected virtual void RightClickPressed()
	{
		if (RightClick != null)
		{
			RightClick();
		}
	}

	protected virtual void ClickedSquareOnGrid (Vector3 pos, RaycastHit squareInfo)
	{
		actionAvailableInSquare = ScanContentsOfSquare(squareInfo, pos);
		if (actionAvailableInSquare != Action.None)
		{
			selectedSquare(pos, squareInfo, actionAvailableInSquare);
		}
	}

	private Action ScanContentsOfSquare(RaycastHit squareInfo, Vector3 pos)
	{               //TODO: not sure which layer name we'll be using yet for valid movement, so I wrote several
		if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "UI") { return Action.None; }
		else if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "Floor") { return Action.Move; }
		else if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "Ground") { return Action.Move; }
		else if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "WalkableArea") { return Action.Move; }
		else return Action.None;
	}

}