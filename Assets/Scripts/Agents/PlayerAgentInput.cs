using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType { None, Moveable, Attackable, Hackable, Talkable, OtherInteractable }
public class PlayerAgentInput : MonoBehaviour
{
	//Image image;
	GridSpace gridSpace;
	ActionType actionAvailable;

	public delegate void MoveSelectionEventHandler(Vector3 pos, RaycastHit squareInfo);
	public delegate void NonMoveSelectionEventHandler();
	public event MoveSelectionEventHandler MoveSelected;
	public event NonMoveSelectionEventHandler NonMoveSelected;

	private void Start()
	{
		//image = GetComponentInChildren<Image>();
		//ShowSelectionMarker(false);
		gridSpace = FindObjectOfType<GridSpace>();
		gridSpace.SquareSelected += OnSquareSelected;
		actionAvailable = ActionType.None;
	}

	//private void ShowSelectionMarker(bool status)
	//{
	//	image.enabled = status;
	//}

	//private void ColorSelectionMarker(ActionType action)
	//{
	//	if (action==ActionType.Moveable) { image.color = Color.green; }
	//}

	private ActionType ScanContentsOfSquare(RaycastHit squareInfo, Vector3 pos)
	{				//TODO: not sure which layer name we'll be using yet for valid movement, so I wrote several
		if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "Floor") { return ActionType.Moveable; }
		else if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "Ground") { return ActionType.Moveable; }
		else if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "WalkableArea") { return ActionType.Moveable; }
		else return ActionType.None;
	}

	protected virtual void OnSquareSelected (Vector3 pos, RaycastHit squareInfo)
	{
		actionAvailable = ScanContentsOfSquare(squareInfo, pos);
		//ShowSelectionMarker(true);
		//ColorSelectionMarker(actionAvailable);
		//this.transform.position = pos;
		if (actionAvailable != ActionType.Moveable && NonMoveSelected != null) { NonMoveSelected(); }
		if (actionAvailable==ActionType.Moveable && MoveSelected != null) { MoveSelected(pos, squareInfo); }
		
	}

}
