using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{

	//control
	private GridSpace m_gridSpace;
	private AgentActionManager m_actionManager;
	private AgentStats m_agentStats;
	private PlayerAgentInput m_agentInput;
	private const float SELECTION_THRESHOLD = 0.4f;
	private bool m_movementSystemInUse = false;
	private Collider[] m_colliders;
	private enum MovementMode { Inactive, MovingAround, PathBuilding }
	private MovementMode currentMode = MovementMode.Inactive;

	//path building
	[SerializeField] private GameObject m_markerPrefab;
	private LineRenderer m_waypointLine;
	private Camera m_cam;
	private Vector3[] m_waypoints;
	private GameObject[] m_markers;
	private float[] m_waypointCosts;

	//path testing
	[SerializeField] private Material m_validPathMaterial;
	[SerializeField] private Material m_farPathMaterial;
	[SerializeField] private Material m_blockedPathMaterial;
	private const float TESTING_WIDTH = 0.95f;
	private LineRenderer m_mouseLine;

	//movement
	private const float MOVE_DEST_THRESHOLD = 0.1f;
	private float m_movementSpeed;
	private float m_MovementPointsAvail = 0f;

    //sound
    private FMOD.Studio.EventInstance walkingEvent;



    private void Start()
	{
		m_cam = Camera.main;
		m_gridSpace = FindObjectOfType<GridSpace>();
		m_agentInput = FindObjectOfType<PlayerAgentInput>();
		m_actionManager = GetComponent<AgentActionManager>();
		m_agentStats = GetComponent<AgentStats>();
		GameObject waypointLineGO = new GameObject();
		GameObject mouseLineGO = new GameObject();
		m_waypointLine = waypointLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		m_mouseLine = mouseLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		m_waypoints = new Vector3[0];
		m_markers = new GameObject[0];
		m_gridSpace.CancelSelected += OnCancelSelected;
		m_agentInput.MoveSelected += OnMovableTileSelected;
		//m_agentInput.NonMoveSelected += OnNonMoveSelected;
		m_colliders = GetComponentsInChildren<Collider>();
        //walkingEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.footSteps2);
    }

	public void MovementActionStarted()     
	{
		m_movementSystemInUse = true;
		m_movementSpeed = m_agentStats.MovementSpeed;
		m_MovementPointsAvail = m_agentStats.MovementPointsPerAction;
		currentMode = MovementMode.PathBuilding;
		m_mouseLine.enabled = true;
		GridSpace.ShowGridSquare(true);
	}

	private void Update()
	{
		if (m_movementSystemInUse==true)
		{
			if (currentMode == MovementMode.Inactive) { return; }
			DrawLineThroughMarkers();
			if (currentMode==MovementMode.PathBuilding)
			{
				if (m_waypoints.Length < 1) { InitializeWaypoints(); }
				m_waypoints[0] = GridSpace.GetGridCoord(transform.position);
				DrawLineToMouse(GetNewestWaypoint());
				return;
			}
			CheckForArrival();
			MoveOnPath();
		}


        if (currentMode == MovementMode.MovingAround)
        {
            
            if (!walkingEvent.IsPlaying())
            {
                Debug.Log("Starting Walking sound");
                walkingEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                walkingEvent.start();
            }
        }
        else
        {
            walkingEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

	private void ResetVariables()
	{
		m_waypoints = new Vector3[0];
		foreach (GameObject marker in m_markers) { Destroy(marker); }
		m_markers = new GameObject[0];
		currentMode = MovementMode.Inactive;
		m_mouseLine.enabled = false;
		m_waypointLine.enabled = false;
		m_movementSystemInUse = false;
		GridSpace.ShowGridSquare(false);
	}

	private void OnMovableTileSelected(Vector3 pos, RaycastHit squareInfo)
	{
		if (currentMode == MovementMode.Inactive) { return; }

		if (GridSpace.GetGridCoord(pos) == GridSpace.GetGridCoord(GetNewestWaypoint())
			&& GridSpace.GetGridCoord(pos) != GridSpace.GetGridCoord(transform.position))
		{
			currentMode = MovementMode.MovingAround;
			return;
		}
		bool valid = TestWaypoint(GetNewestWaypoint(), pos);
		if (!valid) { return; }		
		else 
		{
			float mpCost = Vector3.Distance(GetNewestWaypoint(), pos);
			if (mpCost > m_MovementPointsAvail) { return; }
			PlaceMarker(pos, mpCost);			
			DrawLineThroughMarkers();
		}
	}

	private void InitializeWaypoints()
	{
		m_waypoints = new Vector3[1];
		m_waypoints[0] = GridSpace.GetGridCoord(transform.position);
		m_waypointCosts = new float[1];
		m_waypointCosts[0] = 0f;
		m_markers = new GameObject[1];
		m_markers[0] = null;
	}

	private void PlaceMarker(Vector3 pos, float mpCost)
	{
		if (m_waypoints.Length == 0 || m_waypointCosts.Length == 0) 
		{ 
			InitializeWaypoints(); 
		}
		m_waypoints = ExpandArray<Vector3>(m_waypoints, pos);
		GameObject marker = GameObject.Instantiate(m_markerPrefab, pos, Quaternion.identity);
		marker.transform.Rotate(transform.right, 90f);
		m_markers = ExpandArray<GameObject>(m_markers, marker);
		m_waypointCosts = ExpandArray<float>(m_waypointCosts, mpCost);
		m_MovementPointsAvail = m_MovementPointsAvail - mpCost;
	}

	private Vector3 GetNewestWaypoint()
	{
		if (m_waypoints.Length == 0) { InitializeWaypoints(); }
		return m_waypoints[m_waypoints.Length - 1];
	}
	   
	private T[] ExpandArray<T> (T[] array, T val)
	{
		T[] newArray = new T[array.Length + 1];
		for (int i = 0; i < array.Length; i++) { newArray[i] = array[i]; }
		newArray[newArray.Length - 1] = val;
		return newArray;
	}

	private T[] ContractArray<T>(T[] array)
	{
		T[] newArray = new T[array.Length - 1];
		for (int i = 0; i < array.Length - 1; i++) { newArray[i] = array[i]; }
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
		Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo, 1000f);
		Vector3 mousePos = GridSpace.GetGridCoord(hitInfo.point);
		bool pathIsClear = TestWaypoint(origin, mousePos);
		Material lineColor = m_validPathMaterial;
		if (!pathIsClear) 
		{ 
			lineColor = m_blockedPathMaterial;			//TODO: more consequences than this
		}
		if (DistanceOnPlane(origin, mousePos) > m_MovementPointsAvail) { lineColor = m_farPathMaterial; }	//TODO: more consequences than this
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
		if (m_waypoints.Length < 3) { m_actionManager.ReportActionCancelled(); EndMovement(); return; }
		m_waypoints = ContractArray<Vector3>(m_waypoints);
		GameObject markerToDestroy = m_markers[m_markers.Length-1];
		m_markers = ContractArray<GameObject>(m_markers);
		m_MovementPointsAvail = Mathf.Clamp(m_MovementPointsAvail + m_waypointCosts[m_waypointCosts.Length - 1], 0f, Mathf.Infinity);
		m_waypointCosts = ContractArray<float>(m_waypointCosts);
		Destroy(markerToDestroy);
		DrawLineThroughMarkers();
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
			foreach (Collider myCollider in m_colliders)
			{
				if (hit.collider == myCollider) 
				{
					hitIsOnSelf = true;
				}
			}
			if (!hitIsOnSelf) { hitList.Add(hit); }
		}
		
		foreach (RaycastHit hit in hitList) { Debug.Log("Hit: " + hit.point + "    Obj: " + hit.collider.name); }
	
		if (hitList.Count > 0) { isPathClear = false; }
		return isPathClear;
	}

	public  void EndMovement()
	{
		transform.position = GridSpace.GetGridCoord(transform.position);
		ResetVariables();
		m_actionManager.ReportEndOfAction();
		//reset all path stuff, movement has completed, user cancelled, or user clicked on something else
	}
	
	private float DistanceOnPlane(Vector3 a, Vector3 b)
	{
		a.y = 0f;
		b.y = 0f;
		return Vector3.Distance(a, b);
	}

	private void CheckForArrival()
	{
		if (DistanceOnPlane(transform.position, m_waypoints[1]) < MOVE_DEST_THRESHOLD)
		{
			transform.position = GridSpace.GetGridCoord(m_waypoints[1]);
			RemoveWaypointOnArrival();
			m_waypoints[0] = transform.position;
			DrawLineThroughMarkers();
		}
		if (m_waypoints.Length < 2) 
		{
			EndMovement();
		}
	}

	private void RemoveWaypointOnArrival()
	{
		if (m_waypoints.Length<2) { return; }
		m_waypoints = RemovePositionOneInArray<Vector3>(m_waypoints);
		GameObject markerToDestroy = m_markers[1];
		m_markers = RemovePositionOneInArray<GameObject>(m_markers);
		m_waypointCosts = RemovePositionOneInArray<float>(m_waypointCosts);
		Destroy(markerToDestroy);
	}

	private T[] RemovePositionOneInArray<T>(T[] array)
	{
		
		T[] newArray = new T[array.Length - 1];
		for (int i = 0; i < newArray.Length; i++)
		{
			newArray[i] = array[i+1];
		}
		return newArray;
	}
	

	private void MoveOnPath()
	{
		if (m_waypoints.Length > 1)
		{
			Vector3 dirTowardWaypoint = m_waypoints[1] - transform.position;
			dirTowardWaypoint.y = 0f;
			if (DistanceOnPlane(dirTowardWaypoint, transform.position) > MOVE_DEST_THRESHOLD)
			{
				transform.rotation = Quaternion.LookRotation(dirTowardWaypoint);            //TODO: consider a slower rotation
				transform.position = transform.position + (dirTowardWaypoint.normalized * Time.deltaTime * m_movementSpeed);
			}
			m_waypoints[0] = transform.position;
			DrawLineThroughMarkers();
		}
	}




}
