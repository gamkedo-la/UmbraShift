using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private bool m_canMove = true;
	[SerializeField] private bool m_activeControl = true;
	[SerializeField] private Material m_pathMaterial;
	private NavMeshAgent m_navMeshAgent;
	private GridSpace m_gridSpace;
	private Selection m_selection;
	private GameObject m_pathRenderer;
	private LineRenderer m_path;
	private bool m_showingPathData = false;
	private Vector3 m_destination;
	private int m_remainingWaypoints=0;
	private const float MOVE_DEST_THRESHOLD = 0.2f;
	private const float SELECTION_THRESHOLD = 0.4f;
	private const float SIGHT_THRESHOLD = 5f;
	Vector3[] m_gridpoints;
	private bool m_confirmed = false;
	private bool movingInProcess = false;

	private float _movementSpeed = 3f;  //consider moving this to a player stats script instead
	private float _rotationSpeed = 1f;

	private void Start()
	{
		m_navMeshAgent = GetComponent<NavMeshAgent>();
		m_gridSpace = FindObjectOfType<GridSpace>();
		m_selection = FindObjectOfType<Selection>();
		m_pathRenderer = new GameObject();
		m_path = m_pathRenderer.AddComponent(typeof(LineRenderer)) as LineRenderer;
		m_destination = Vector3.positiveInfinity;
		m_gridpoints = new Vector3[0];
		m_selection.MoveSelected += OnMoveSelected;
		m_selection.NonMoveSelected += OnNonMoveSelected;
	}

	private void OnMoveSelected(Vector3 pos, RaycastHit squareInfo)
	{
		if (movingInProcess || !m_activeControl || !m_canMove) { return; }
		if (DistanceOnPlane(pos, m_destination) < SELECTION_THRESHOLD)
		{
			m_confirmed = true;
		}
		else { m_confirmed = false; }
		if (m_confirmed == true) { BeginMovingOnPath(); }
		else { CreateNewPath(pos); }
	}

	private void OnNonMoveSelected()
	{
		EndMovement();
	}

	private void EndMovement()
	{
		movingInProcess = false;
		m_showingPathData = false;
		m_confirmed = false;
		m_destination = Vector3.positiveInfinity;
		m_gridpoints = new Vector3[0];
		transform.position = m_gridSpace.GetGridCoord(transform.position);
		//reset all path stuff, movement has completed, user cancelled, or user clicked on something else
	}

	private void Update()
	{
		if (!m_activeControl || !m_canMove) { return; }
		if (!movingInProcess) { return; }
		CheckForArrival();
		if (!movingInProcess) { return; }
		DrawLinesAlongGridpoints();
		MoveOnPath();
	}

	private void CheckForArrival()
	{
		Debug.Log("Gridpoints: " + m_gridpoints[0]);
		if (DistanceOnPlane(transform.position, m_gridpoints[0]) < MOVE_DEST_THRESHOLD)
		{
			RemoveNextGridpoint();
		}
		if (DistanceOnPlane(transform.position, m_destination) < MOVE_DEST_THRESHOLD)
		{
			EndMovement();
		}
	}

	private void RemoveNextGridpoint()
	{
		Vector3[] tempArray = new Vector3[m_gridpoints.Length - 1];
		for (int i=0; i<tempArray.Length; i++)
		{
			tempArray[i] = m_gridpoints[i+1];
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
			gridpoints[i-1] = m_gridSpace.GetGridCoord(waypoints[i]);
		}
		return gridpoints;
	}

	private float DistanceOnPlane (Vector3 a, Vector3 b)
	{
		a.y = 0f;
		b.y = 0;
		return Vector3.Distance(a, b);
	}

	private void DrawLinesAlongGridpoints()
	{
		m_path.enabled = true;
		Vector3[] positions = new Vector3[m_gridpoints.Length + 1];
		for (int i=1; i<positions.Length; i++)
		{
			positions[i] = m_gridpoints[i-1];
		}
		positions[0] = transform.position;
		m_pathMaterial.color = Color.green;     //TODO: color change isn't working
		m_path.material = m_pathMaterial;
		m_path.startWidth = 0.1f;
		m_path.endWidth = 0.1f;
		m_path.positionCount = positions.Length;
		m_path.SetPositions(positions);
		m_path.useWorldSpace = true;
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

	
	

}
