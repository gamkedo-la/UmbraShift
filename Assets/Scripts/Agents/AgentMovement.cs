using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
	//control
	[SerializeField] private bool m_canMove = true;
	[SerializeField] private bool m_activeControl = true;
	private bool m_movementActivated = false;
	private GridSpace m_gridSpace;
	private PlayerAgentInput m_player_input;
	private const float SELECTION_THRESHOLD = 0.4f;
	private bool m_confirmed = false;
	private bool movingInProcess = false;
	private bool settingPathInProcess = true;
	private Collider[] m_colliders;

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
	private float[] m_waypointCosts;

	//movement
	private const float MOVE_DEST_THRESHOLD = 0.1f;
	private float _movementSpeed = 5f;  //consider moving this to a player stats script instead
	private float _rotationSpeed = 1f;
	private float m_MovePntsPerAction = 500f;   //Amount set high for Demo & Debug purposes only
	private float m_MovementPointsAvail = 0f;

    //sound
    private FMOD.Studio.EventInstance walkingEvent;



    private void Start()
	{
		cam = Camera.main;
		m_gridSpace = FindObjectOfType<GridSpace>();
		m_player_input = FindObjectOfType<PlayerAgentInput>();
		GameObject waypointLineGO = new GameObject();
		GameObject mouseLineGO = new GameObject();
		m_waypointLine = waypointLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		m_mouseLine = mouseLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		m_waypoints = new Vector3[0];
		m_markers = new GameObject[0];
		m_player_input.MoveSelected += OnMoveSelected;
		m_player_input.NonMoveSelected += OnNonMoveSelected;
		m_gridSpace.CancelSelected += OnCancelSelected;
		m_colliders = GetComponentsInChildren<Collider>();
        walkingEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.footSteps2);


    }

	private void ActionUsedForMovement()          //Demo & Debug purposes only
	{
		if (m_movementActivated == false)
		{
			m_MovementPointsAvail = m_MovePntsPerAction;
			m_movementActivated = true;
			settingPathInProcess = true;
		}
		else if (m_movementActivated == true) 
		{
			m_MovementPointsAvail = 0;
			m_movementActivated = false;
			settingPathInProcess = false;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) { ActionUsedForMovement(); }
		if (m_movementActivated)
		{
			if (!m_activeControl || !m_canMove) { return; }
			DrawLineThroughMarkers();
			if (!movingInProcess && settingPathInProcess)
			{
				if (m_waypoints.Length < 1) { InitializeWaypoints(); }
				m_waypoints[0] = GridSpace.GetGridCoord(transform.position);
				DrawLineToMouse(GetNewestWaypoint());
				return;
			}
			CheckForArrival();
			MoveOnPath();
		}


        if (movingInProcess)
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
		m_confirmed = false;
		movingInProcess = false;
		settingPathInProcess = false;
		m_mouseLine.enabled = false;
		m_waypointLine.enabled = false;
		m_movementActivated = false;
	}

	private void OnMoveSelected(Vector3 pos, RaycastHit squareInfo)
	{
		if (!settingPathInProcess || !m_activeControl || !m_canMove) { return; }

		if (GridSpace.GetGridCoord(pos) == GridSpace.GetGridCoord(GetNewestWaypoint())
			&& GridSpace.GetGridCoord(pos) != GridSpace.GetGridCoord(transform.position))
		{
			BeginMovingOnPath();
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
		GameObject marker = GameObject.Instantiate(markerPrefab, pos, Quaternion.identity);
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
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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
		if (m_waypoints.Length < 3) { EndMovement(); return; }
		m_waypoints = ContractArray<Vector3>(m_waypoints);
		GameObject markerToDestroy = m_markers[m_markers.Length-1];
		m_markers = ContractArray<GameObject>(m_markers);
		m_MovementPointsAvail = Mathf.Clamp(m_MovementPointsAvail + m_waypointCosts[m_waypointCosts.Length - 1], 0f, m_MovePntsPerAction);
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
		RaycastHit[] hitArray = Physics.SphereCastAll(startPosition, 0.35f, ray, ray.magnitude);
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

	private void BeginMovingOnPath()
	{
		movingInProcess = true;
		settingPathInProcess = false;
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
				transform.position = transform.position + (dirTowardWaypoint.normalized * Time.deltaTime * _movementSpeed);
			}
			DrawLineThroughMarkers();
		}
	}




}
