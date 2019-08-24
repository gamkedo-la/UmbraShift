using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType { None, Moveable, Attackable, Hackable, Talkable, OtherInteractable }
public enum ActionInProcess { None, Moving, Shooting, Hacking, Talking, Investigating }
public class PlayerAgentInput : MonoBehaviour
{
	private GridSpace gridSpace;
	private GameManager gameManager;
	private ActionType actionAvailable;
	public ActionInProcess actionInProcess = ActionInProcess.None;
	public delegate void MoveSelectionEventHandler(Vector3 pos, RaycastHit squareInfo);
	public delegate void NonMoveSelectionEventHandler();
	public delegate void PortraitButtonPressEventHandler();
	public delegate void MoveHotkeyPressEventHandler();
	public event MoveSelectionEventHandler MoveSelected;
	public event NonMoveSelectionEventHandler NonMoveSelected;
	public event PortraitButtonPressEventHandler PortraitButtonPressed;
	public event MoveHotkeyPressEventHandler MoveHotkeyWasPressed;



	private void Start()
	{
		//image = GetComponentInChildren<Image>();
		//ShowSelectionMarker(false);
		gridSpace = FindObjectOfType<GridSpace>();
		gridSpace.SquareSelected += OnSquareSelected;
		actionAvailable = ActionType.None;
		gameManager = FindObjectOfType<GameManager>();
	}

	private void Update()
	{
		if (actionInProcess == ActionInProcess.None)
		{
			if (Input.GetKeyDown(KeyCode.M)) 
			{
				OnMoveHotkeyPressed();
			} 
		}
	}

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

	public void OnPortraitButtonPressed()
	{
		PortraitButtonPressed();
	}

	protected virtual void OnMoveHotkeyPressed()
	{
		MoveHotkeyWasPressed();
	}

	public void InitiateMoveAction()
	{
		actionInProcess = ActionInProcess.Moving;
		gameManager.ActiveCharacter.GetComponent<AgentActionManager>().OnMoveActionSelected();
	}

}
