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
	private AgentTurnManager turnManager;
	private UIActionBarController actionBarController;
	private ActionType actionAvailable;
	public ActionInProcess actionInProcess = ActionInProcess.None;
	public delegate void SelectionEventHandler(Vector3 pos, RaycastHit squareInfo);
	public event SelectionEventHandler selectedSquare;
	private bool inputLocked = false;
	public bool InputLocked { get { return inputLocked; } set { inputLocked = value; } }
	
	public void LockInput(bool locking)
	{
		inputLocked = locking;
	}
	
	private void Start()
	{
		actionBarController = FindObjectOfType<UIActionBarController>();
		gridSpace = FindObjectOfType<GridSpace>();
		gridSpace.SquareSelected += OnSquareSelected;
		actionAvailable = ActionType.None;
		turnManager = FindObjectOfType<AgentTurnManager>();
	}

	private void Update()
	{
		if (actionInProcess == ActionInProcess.None)
		{
			if (Input.GetKeyDown(KeyCode.M)) 
			{
				actionBarController.OnMoveActionButtonPressed();
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
		//if (actionAvailable != ActionType.Moveable && NonMoveSelected != null) { NonMoveSelected(); }
		//if (actionAvailable==ActionType.Moveable && gridSpaceSelected != null) { gridSpaceSelected(pos, squareInfo); }
	}

	public void InitiateMoveAction()
	{
		actionInProcess = ActionInProcess.Moving;
		turnManager.ActiveCharacter.GetComponent<AgentActionManager>().OnMoveActionSelected();
	}

}
