using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
	[Header("Mouse lines")]
	[SerializeField] private GameObject waypointPrefab;
	[SerializeField] private Material m_validPathMaterial;
	[SerializeField] private Material m_blockedPathMaterial;
	[SerializeField] private Material m_farPathMaterial;

	private enum MovementMode { Inactive, PathBuilding, MovingAround }
	private MovementMode movementMode = MovementMode.Inactive;
	private List<Transform> waypointsPlaced;
	private List<float> waypointCosts;
	private AgentTurnManager turnManager;
	private float movePointsAvailable = 0;
	private float movementSpeed = 0;
	private Vector3 originalPositionAtActionStart;
	private Collider[] colliders;
	private Camera cameraForRaycastingToMouse;
	private const float TESTING_WIDTH = 0.95f;
	private const float ARRIVAL_THRESHOLD = 0.1f;
	private LineRenderer waypointLine;
	private LineRenderer mouseLine;
	private GridSelectionController selectionController;
    public Material defaultMaterial;

    private void Start()
	{
		turnManager = FindObjectOfType<AgentTurnManager>();
		colliders = GetComponentsInChildren<Collider>();
		cameraForRaycastingToMouse = Camera.main;
		GameObject waypointLineGO = new GameObject();
		GameObject mouseLineGO = new GameObject();
		waypointLine = waypointLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
        waypointLine.GetComponent<LineRenderer>().enabled = false;
		mouseLine = mouseLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
        mouseLine.GetComponent<LineRenderer>().enabled = false;
        selectionController = FindObjectOfType<GridSelectionController>();
		selectionController.selectedSquare += SquareSelected;
		waypointsPlaced = new List<Transform>();
		waypointCosts = new List<float>();
	}

	private void Update()
	{
		if (movementMode == MovementMode.PathBuilding) { PathBuildingUpdate(); }
		if (movementMode == MovementMode.MovingAround) { MovingAroundUpdate(); }
	}

	private void RefreshMovementPoints()
	{
		movePointsAvailable = turnManager.ActiveCharacter.MovementPointsPerAction;
	}

	private float DetermineSpeed()
	{
		return turnManager.ActiveCharacter.MovementSpeed;
	}

	private void ResetVariables()	///////////////////////////////
	{
		movePointsAvailable = 0;
		movementSpeed = 0f;
		movementMode = MovementMode.Inactive;
		if (waypointsPlaced.Count > 0) 
		{
			foreach (Transform waypoint in waypointsPlaced) { Destroy(waypoint.gameObject); }
		}
		waypointsPlaced.Clear();
		waypointCosts.Clear();
		waypointLine.enabled = false;
		mouseLine.enabled = false;
		GridSpace.ShowGridSquare(false);
	}

	public void EndMovement()
	{
		
	}

	public void ActionStarted()
	{
		ResetVariables();
		transform.position = GridSpace.GetGridCoord(transform.position);
		originalPositionAtActionStart = transform.position;
		movementSpeed = DetermineSpeed();
		RefreshMovementPoints();
		BeginPathBuilding();
	}

	private void BeginPathBuilding()
	{
		GridSpace.ShowGridSquare(true);
		movementMode = MovementMode.PathBuilding;
		mouseLine.enabled = true;
		waypointLine.enabled = true;
	}

	private void BeginMovingAround()
	{
		GridSpace.ShowGridSquare(false);
		movementMode = MovementMode.MovingAround;
		mouseLine.enabled = false;
	}

	public void Undo()
	{
		if (waypointsPlaced.Count > 0 && movementMode == MovementMode.PathBuilding)
		{
			int max = waypointsPlaced.Count - 1;
			Transform waypoint = waypointsPlaced[max];
			waypointsPlaced.RemoveAt(max);
			PayMovePoints(-waypointCosts[max]);
			waypointCosts.RemoveAt(max);
			Destroy(waypoint.gameObject);
		}
	}

	public void ActionCancel()
	{
		ResetVariables();
		GridSpace.ShowGridSquare(false);
		EndMovement();
	}

	public void ActionContinue()
	{
		if (waypointsPlaced.Count > 0 && movementMode == MovementMode.PathBuilding)
		{
			BeginMovingAround();
		}
	}

	public void ActionComplete()
	{
		if (movementMode == MovementMode.PathBuilding)
		{
			BeginMovingAround();
		}
		else if (movementMode == MovementMode.MovingAround)
		{
			ResetVariables();
			GridSpace.ShowGridSquare(false);
			EndMovement();
			turnManager.ActiveCharacter.actionManager.ReportEndOfAction();
		}
	}

	private void PathBuildingUpdate()
	{
		DrawLineThroughWaypoints(); 
		DrawLineToMouse();
	}

	private void MovingAroundUpdate()
	{
		DrawLineThroughWaypoints();
		CheckForArrivalAtWaypoint();
		RotateTowardNextWaypoint();
		MoveTowardNextWaypoint();
		CheckForMovementComplete();
	}

	private void CheckForArrivalAtWaypoint()
	{
		float dist = DistanceOnPlane(transform.position, waypointsPlaced[0].position);
		if (dist < ARRIVAL_THRESHOLD)
		{
			Transform waypoint = waypointsPlaced[0];
			waypointsPlaced.RemoveAt(0);
			waypointCosts.RemoveAt(0);
			Destroy(waypoint.gameObject);
		}
	}

	private void RotateTowardNextWaypoint()
	{
		if (waypointsPlaced.Count > 0)
		{ transform.LookAt(waypointsPlaced[0]); };
	}

	private void MoveTowardNextWaypoint()
	{
		if (waypointsPlaced.Count > 0)
		{
			float dist = DistanceOnPlane(transform.position, waypointsPlaced[0].position);
			float moveDist = Mathf.Clamp(movementSpeed * Time.deltaTime, 0, dist);
			Vector3 moveVector = (waypointsPlaced[0].position - transform.position) * moveDist;
			Vector3 pos = transform.position + moveVector;
			transform.position = pos;
		}
	}

	private void CheckForMovementComplete()
	{
		if (waypointsPlaced.Count == 0)
		{
			transform.position = GridSpace.GetGridCoord(transform.position);
			ActionComplete();
		}
	}


	private void SquareSelected(Vector3 pos, RaycastHit squareInfo, Action actionAvailableInSquare)
	{
		if (movementMode == MovementMode.PathBuilding) { SquareSelectedWhilePathBuilding(pos, squareInfo, actionAvailableInSquare); }
		if (movementMode == MovementMode.MovingAround) { SquareSelectedWhileMovingAround(pos, squareInfo, actionAvailableInSquare); }
	}

	private void SquareSelectedWhilePathBuilding(Vector3 pos, RaycastHit squareInfo, Action actionAvailableInSquare)
	{
		if (actionAvailableInSquare == Action.Move) 
		{
			Vector3 latestWaypointPos = GetLatestWaypointPos();
			bool valid = TestWaypoint(latestWaypointPos, pos);
			float cost = DistanceOnPlane(latestWaypointPos, pos);
			bool inMoveBudget = movePointsAvailable >= cost;
			if (valid && GridSpace.GetGridCoord(pos) == GridSpace.GetGridCoord(GetLatestWaypointPos())) 
			{
				BeginMovingAround();
				return; 
			}
			if (valid && inMoveBudget) { AttemptToPlaceWaypoint(pos); }
		}
	}

	private void SquareSelectedWhileMovingAround(Vector3 pos, RaycastHit squareInfo, Action actionAvailableInSquare)
	{
		//do nothing?
	}

	private void DrawLineToMouse()
	{
		Ray ray = cameraForRaycastingToMouse.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo, 1000f);
		Vector3 mousePos = GridSpace.GetGridCoord(hitInfo.point);
		Vector3 origin;
		if (waypointsPlaced.Count > 0) { origin = waypointsPlaced[waypointsPlaced.Count - 1].position; }
		else { origin = GridSpace.GetGridCoord(transform.position); }
		bool pathIsClear = TestWaypoint(origin, mousePos);
		Material lineColor = m_validPathMaterial;
		if (!pathIsClear)
		{
			lineColor = m_blockedPathMaterial;
		}
		if (DistanceOnPlane(origin, mousePos) > movePointsAvailable) { lineColor = m_farPathMaterial; }
		mouseLine.material = lineColor;
		mouseLine.startWidth = 0.05f;
		mouseLine.endWidth = 0.05f;
		mouseLine.positionCount = 2;
		Vector3[] positions = new Vector3[2];
		positions[0] = origin;
		positions[1] = mousePos;
		mouseLine.SetPositions(positions);
		mouseLine.useWorldSpace = true;
	}

	private void DrawLineThroughWaypoints()
	{
		waypointLine.material = m_validPathMaterial;
		waypointLine.startWidth = 0.05f;
		waypointLine.endWidth = 0.05f;
		waypointLine.positionCount = waypointsPlaced.Count+1;
		List<Vector3> positions = new List<Vector3>();
		positions.Add(transform.position);
		foreach (Transform waypoint in waypointsPlaced) 
		{
			positions.Add(waypoint.position);
		}
		Vector3[] positionsArray = positions.ToArray();
		waypointLine.SetPositions(positionsArray);
		waypointLine.useWorldSpace = true;
	}


	private bool TestWaypoint(Vector3 startPosition, Vector3 testPosition)
	{
		bool isPathClear = true;
		startPosition.y = testPosition.y + 1;
		testPosition.y = testPosition.y + 1;
		Vector3 ray = testPosition - startPosition;
		RaycastHit[] hitArray = Physics.SphereCastAll(startPosition, TESTING_WIDTH, ray, ray.magnitude);
		List<RaycastHit> hitList = new List<RaycastHit>();

		foreach (RaycastHit hit in hitArray)
		{
			bool hitIsOnSelf = false;
			foreach (Collider myCollider in colliders)
			{
				if (hit.collider == myCollider)
				{
					hitIsOnSelf = true;
				}
			}
			if (!hitIsOnSelf) { hitList.Add(hit); }
		}

		if (hitList.Count > 0) { isPathClear = false; }
		return isPathClear;
	}

	private float DistanceOnPlane(Vector3 a, Vector3 b)
	{
		a.y = 0f;
		b.y = 0f;
		return Vector3.Distance(a, b);
	}

	private void AttemptToPlaceWaypoint(Vector3 pos)
	{
		Vector3 latestWaypointPos = GetLatestWaypointPos();
		pos = GridSpace.GetGridCoord(pos);
		float moveCost = DistanceOnPlane(latestWaypointPos, pos);
		if (movePointsAvailable >= moveCost)
		{
			PayMovePoints(moveCost);
			waypointCosts.Add(moveCost);
			GameObject waypoint = Instantiate(waypointPrefab, pos, Quaternion.LookRotation(cameraForRaycastingToMouse.transform.position - pos));
			waypoint.name = "Waypoint";
			waypointsPlaced.Add(waypoint.transform);
		}
	}

	private void PayMovePoints(float cost)
	{
		movePointsAvailable -= cost;
	}

	private Vector3 GetLatestWaypointPos()
	{
		Vector3 pos;
		if (waypointsPlaced.Count > 0) { pos = waypointsPlaced[waypointsPlaced.Count - 1].position; }
		else { pos = transform.position; }
		return GridSpace.GetGridCoord(pos);
	}




}
