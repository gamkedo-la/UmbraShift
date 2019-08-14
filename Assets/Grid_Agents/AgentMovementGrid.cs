using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovementGrid : MonoBehaviour
{
	//control
	[SerializeField] private bool m_canMove = true;
	[SerializeField] private bool m_activeControl = true;
	private GridSpace m_gridSpace;
	private PlayerInputGrid m_player_input;
	private const float SELECTION_THRESHOLD = 0.4f;
	private bool m_confirmed = false;
	private bool movingInProcess = false;
	private bool settingPathInProcess = true;

	//lines
	[SerializeField] private Material m_validPathMaterial;
	[SerializeField] private Material m_farPathMaterial;
	[SerializeField] private Material m_blockedPathMaterial;
	private LineRenderer m_waypointLine;
	private LineRenderer m_mouseLine;
	private Camera cam;

	//waypoints
	[SerializeField] private GameObject markerPrefab;
	private Vector3[] m_waypoints;
	private GameObject[] m_markers;

	//movement
	private const float MOVE_DEST_THRESHOLD = 0.1f;
	private float _movementSpeed = 3f;  //consider moving this to a player stats script instead
	private float _rotationSpeed = 1f;
	private float _MaxMovementPoints = 6f;
	private float _MovementPointsAvail = 6f;


	private void Start()
	{
		cam = Camera.main;
		m_gridSpace = FindObjectOfType<GridSpace>();
		m_player_input = FindObjectOfType<PlayerInputGrid>();
		GameObject waypointLineGO = new GameObject();
		GameObject mouseLineGO = new GameObject();
		m_waypointLine = waypointLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		m_mouseLine = mouseLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		m_waypoints = new Vector3[0];
		m_markers = new GameObject[0];
		m_player_input.MoveSelected += OnMoveSelected;
		m_player_input.NonMoveSelected += OnNonMoveSelected;
		m_gridSpace.CancelSelected += OnCancelSelected;
	}

	private void ResetVariables()
	{
		m_waypoints = new Vector3[0];
		foreach (GameObject marker in m_markers) { Destroy(marker); }
		m_markers = new GameObject[0];
		m_confirmed = false;
		movingInProcess = false;
		settingPathInProcess = false;
		m_mouseLine.enabled = false;
		m_waypointLine.enabled = false;
	}

	private void OnMoveSelected(Vector3 pos, RaycastHit squareInfo)
	{
		if (movingInProcess || !m_activeControl || !m_canMove) { return; }
		
		if (m_waypoints.Length<1) { InitializeWaypoints(); }
		bool valid = TestWaypoint(m_waypoints[m_waypoints.Length-1], pos);
		if (!valid) { return; }		//or draw invalid path
		else 
		{
			PlaceMarker(pos);
			DrawLineThroughMarkers();
		}
	}

	private void InitializeWaypoints()
	{
		m_waypoints = new Vector3[1];
		m_waypoints[0] = GridSpace.GetGridCoord(transform.position);
	}

	private void PlaceMarker(Vector3 pos)
	{
		if (m_waypoints.Length == 0) { AddNewPosToAnArray(m_waypoints, GridSpace.GetGridCoord(transform.position)); }
		m_waypoints = AddNewPosToAnArray(m_waypoints, pos);
		GameObject marker = GameObject.Instantiate(markerPrefab, pos, Quaternion.identity, this.transform);
		
	}
	   
	private GameObject[] AddNewMarkerToAnArray(GameObject[] array, GameObject marker)
	{
		GameObject[] newArray = new GameObject[array.Length + 1];
		for (int i = 0; i < array.Length; i++) { newArray[i] = array[i]; }
		newArray[newArray.Length - 1] = marker;
		return newArray;
	}

	private GameObject[] RemoveLastMarkerFromAnArray(GameObject[] array)
	{
		GameObject[] newArray = new GameObject[array.Length - 1];
		for (int i = 0; i < array.Length - 1; i++) { newArray[i] = array[i]; }
		return newArray;
	}

	private Vector3[] AddNewPosToAnArray(Vector3[] array, Vector3 pos)
	{
		Vector3[] newArray = new Vector3[array.Length + 1];
		for (int i = 0; i < array.Length; i++) { newArray[i] = array[i]; }
		newArray[newArray.Length-1] = pos;
		return newArray;
	}

	private Vector3[] RemoveLastPosFromAnArray(Vector3[] array)
	{
		Vector3[] newArray = new Vector3[array.Length - 1];
		for (int i = 0; i < array.Length-1; i++) { newArray[i] = array[i]; }
		return newArray;
	}

	private void DrawLineThroughMarkers()
	{
		m_waypointLine.enabled = true;
		m_waypointLine.material = m_validPathMaterial;
		m_waypointLine.startWidth = 0.05f;
		m_waypointLine.endWidth = 0.05f;
		m_waypointLine.positionCount = m_waypoints.Length;
		m_waypointLine.SetPositions(m_waypoints);
		m_waypointLine.useWorldSpace = true;
	}

	private void DrawLineToMouse(Vector3 origin)
	{
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo, 100f);
		Vector3 mousePos = GridSpace.GetGridCoord(hitInfo.point);
		bool pathIsClear = TestWaypoint(origin, mousePos);
		Material lineColor = m_validPathMaterial;
		if (!pathIsClear) { lineColor = m_blockedPathMaterial; }
		if (Vector3.Distance(origin, mousePos) > _MovementPointsAvail) { lineColor = m_farPathMaterial; }
		m_mouseLine.material = lineColor;
		m_mouseLine.startWidth = 0.05f;
		m_mouseLine.endWidth = 0.05f;
		m_mouseLine.positionCount = 2;
		Vector3[] positions = new Vector3[2];
		positions[0] = origin;
		positions[1] = mousePos;
		m_mouseLine.SetPositions(positions);
		m_mouseLine.useWorldSpace = true;
	}

	private void OnCancelSelected()
	{
		if (m_waypoints.Length < 3) { EndMovement(); return; }
		m_waypoints = RemoveLastPosFromAnArray(m_waypoints);
		GameObject markerToDestroy = m_markers[m_markers.Length];
		m_markers = RemoveLastMarkerFromAnArray(m_markers);
		Destroy(markerToDestroy);
	}

	private bool TestWaypoint(Vector3 startPosition, Vector3 testPosition)
	{
		bool isPathClear = true;
		int layersToIgnore = LayerMask.GetMask("Player");
		startPosition.y = testPosition.y + 1;
		testPosition.y = testPosition.y + 1;
		RaycastHit hitInfo;
		bool hit = Physics.SphereCast(startPosition, 0.45f, testPosition - startPosition, out hitInfo, (testPosition - startPosition).magnitude);
		if (hit) { isPathClear = false; }
		return isPathClear;
	}



	private void OnNonMoveSelected()
	{
		EndMovement();
	}

	private void EndMovement()
	{
		ResetVariables();
		transform.position = GridSpace.GetGridCoord(transform.position);
		//reset all path stuff, movement has completed, user cancelled, or user clicked on something else
	}

	private void Update()
	{
		if (!m_activeControl || !m_canMove) { return; }
		if (!movingInProcess) { return; }
		DrawLineToMouse(m_waypoints[m_waypoints.Length-1]);
		//CheckForArrival();
		//if (!movingInProcess) { return; }
		//MoveOnPath();
	}

	/*
	private void CheckForArrival()
	{
		if (DistanceOnPlane(transform.position, m_gridpoints[0]) < MOVE_DEST_THRESHOLD)
		{
			RemoveNextGridpoint();
			transform.position = m_gridSpace.GetGridCoord(transform.position);
		}
		if (DistanceOnPlane(transform.position, m_destination) < MOVE_DEST_THRESHOLD)
		{
			EndMovement();
			transform.position = m_gridSpace.GetGridCoord(transform.position);
		}
	}

	private void RemoveNextGridpoint()
	{
		Vector3[] tempArray = new Vector3[m_gridpoints.Length - 1];
		for (int i = 0; i < tempArray.Length; i++)
		{
			tempArray[i] = m_gridpoints[i + 1];
		}
		m_gridpoints = tempArray;
	}

	private void CreateNewPath(Vector3 dest)
	{
		m_showingPathData = true;
		m_destination = dest;
		NavMeshPath path = CalcNavMeshPathTo(dest);
		m_gridpoints = CalcGridpointsFromNavMeshPath(path);
		DrawLinesAlongGridpoints();
	}

	private void BeginMovingOnPath()
	{
		movingInProcess = true;
	}

	private NavMeshPath CalcNavMeshPathTo(Vector3 destGridCoord)
	{
		NavMeshPath pathToCalc = new NavMeshPath();
		NavMesh.CalculatePath(transform.position, destGridCoord, NavMesh.AllAreas, pathToCalc);       //consider adding some area masks
		return pathToCalc;
	}

	private Vector3[] CalcGridpointsFromNavMeshPath(NavMeshPath path)
	{
		Vector3[] waypoints = path.corners;
		Vector3[] gridpoints = new Vector3[waypoints.Length - 1];
		for (int i = 1; i < waypoints.Length; i++)
		{
			gridpoints[i - 1] = m_gridSpace.GetGridCoord(waypoints[i]);
		}
		return gridpoints;
	}

	private float DistanceOnPlane(Vector3 a, Vector3 b)
	{
		a.y = 0f;
		b.y = 0;
		return Vector3.Distance(a, b);
	}

	

	private void MoveOnPath()
	{
		Vector3 dirTowardWaypoint = m_gridpoints[0] - transform.position;
		dirTowardWaypoint.y = 0f;
		if (DistanceOnPlane(dirTowardWaypoint, transform.position) > MOVE_DEST_THRESHOLD)
		{
			transform.rotation = Quaternion.LookRotation(dirTowardWaypoint);
			transform.position = transform.position + (dirTowardWaypoint.normalized * Time.deltaTime * _movementSpeed);
		}
	}
	*/
	


}
