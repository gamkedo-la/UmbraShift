using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ActionType { None, Moveable, Attackable, Hackable, Talkable, OtherInteractable }
public class Selection : MonoBehaviour
{
	Image image;
	GridSpace gridSpace;
	ActionType actionAvailable;
	private TextMeshProUGUI text;

	public delegate void MoveSelectionEventHandler(Vector3 pos, RaycastHit squareInfo);
	public event MoveSelectionEventHandler MoveSelected;


	private void Start()
	{
		image = GetComponentInChildren<Image>();
		text = GetComponentInChildren<TextMeshProUGUI>();
		ShowSelectionMarker(false);
		gridSpace = FindObjectOfType<GridSpace>();
		gridSpace.SquareSelected += OnSquareSelected;
	}

	private void ShowSelectionMarker(bool status)
	{
		image.enabled = status;
		text.enabled = status;
		text.transform.rotation = Camera.main.transform.rotation;
	}

	private void ColorSelectionMarker(ActionType action)
	{
		if (action==ActionType.Moveable) { image.color = Color.green; }
	}

	private void SetSelectionText (int apCost) 
	{
		text.text = apCost.ToString();
	}
    
	private ActionType ScanContentsOfSquare(RaycastHit squareInfo, Vector3 pos)
	{
		if (LayerMask.LayerToName(squareInfo.collider.gameObject.layer) == "Floor") { return ActionType.Moveable; }
		else return ActionType.None;
	}

	protected virtual void OnSquareSelected (Vector3 pos, RaycastHit squareInfo)
	{
		actionAvailable = ScanContentsOfSquare(squareInfo, pos);
		ShowSelectionMarker(true);
		SetSelectionText(UnityEngine.Random.Range(0,4));		//TODO: Get actual AP cost data instead of a random number
		ColorSelectionMarker(actionAvailable);
		this.transform.position = pos;
		if (actionAvailable==ActionType.Moveable && MoveSelected != null) { MoveSelected(pos, squareInfo); }
		
	}
}
