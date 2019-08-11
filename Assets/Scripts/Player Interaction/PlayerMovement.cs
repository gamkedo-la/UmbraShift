using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private bool canMove = true;
	[SerializeField] private bool activeControl = true;
	[SerializeField] private Material pathMaterial;
	private NavMeshAgent navMeshAgent;
	private GridSpace gridSpace;
	private Selection selection;
	private GameObject myPath;
	private bool showingPathData = false;
	private Vector3 moveDestination;
	private const float MOVE_DEST_THRESHOLD = 0.2f;
	
	private void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		gridSpace = FindObjectOfType<GridSpace>();
		selection = FindObjectOfType<Selection>();
		selection.MoveSelected += OnMoveSelected;
		myPath = new GameObject();
		myPath.AddComponent(typeof(LineRenderer));
	}

	private void ResetPath()
	{
		myPath.GetComponent<LineRenderer>().enabled = false;
	}

	private void OnMoveSelected(Vector3 pos, RaycastHit squareInfo)
	{
		if (!activeControl || !canMove) { return; }

		if (showingPathData==true) { ResetPath(); }
		showingPathData = true;
		NavMeshPath route = CalcRouteToDest(pos);
		Vector3[] waypoints = route.corners;
		Vector3[] gridpoints = new Vector3[waypoints.Length+1];
		gridpoints[0] = gridSpace.GetGridCoord(transform.position);
		for (int i=0; i<waypoints.Length; i++) 
		{ 
			gridpoints[i+1] = gridSpace.GetGridCoord(waypoints[i]); 
		}
		DrawLines(myPath, gridpoints);
	}

	private void DrawLines (GameObject pathGO, Vector3[] gridpoints)
	{
		LineRenderer line = myPath.GetComponent<LineRenderer>();
		line.enabled = true;
		pathMaterial.color = Color.green;		//TODO: not working
		line.material = pathMaterial;
		line.startWidth = 0.1f;
		line.endWidth = 0.1f;
		line.positionCount = gridpoints.Length;
		line.SetPositions(gridpoints);
		line.useWorldSpace = true;
	}

	private NavMeshPath CalcRouteToDest(Vector3 destPos)
	{
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(transform.position, destPos, NavMesh.AllAreas, path);
		Debug.Log(path.corners);
		return path;
	}

	




}
