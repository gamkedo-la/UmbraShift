using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteracting : MonoBehaviour
{
	[SerializeField] Material lineMaterial;
	[SerializeField] private GameObject torusPrefab;
	private GameObject torus;
	private bool interactingSystemInUse = false;
	public bool InteractingSystemInUse { get { return interactingSystemInUse; } }
	private AgentTurnManager turnManager;
	private List<Targetable> targetList;
	private Targetable closestTargetNearMouse;
	private Targetable targetLocked;
	private Color clearLOSColor = Color.green;
	private Color noLOSColor = Color.red;
	private const float INTERACTION_RANGE = 6f;	
	private Vector3 targetedPoint;
	private Vector3 mousePoint;
	private Vector3 impactPoint;
	private Camera cameraForRaycastingToMouse;
	private LineRenderer mouseLine;
	private const float SNAPPING_DISTANCE = 1.5f;
	private bool rotatingNow = false;
	private Collider[] selfColliders;
	Targetable selfTarget;
	private bool targetLockisHeld = false;
	private float FIELD_OF_VIEW = 30f;
	private bool interacting = false;

	private void Start()
    {
		turnManager = FindObjectOfType<AgentTurnManager>();
		selfTarget = gameObject.GetComponent<Targetable>();
		targetList = new List<Targetable>();
		cameraForRaycastingToMouse = Camera.main;
		selfColliders = this.gameObject.GetComponentsInChildren<Collider>();
		GameObject mouseLineGO = new GameObject();
		mouseLine = mouseLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		mouseLine.material = lineMaterial;
		mouseLine.enabled = false;
	}

	private void CreateDetectionTorus(bool isActive)
	{
		torus = Instantiate(torusPrefab, selfTarget.TargetPos, Quaternion.identity, this.transform);
		Vector3 torusScale = torus.transform.localScale;
		Vector3 torusPos = selfTarget.TargetPos;
		torusScale.x = INTERACTION_RANGE * 2;
		torusScale.z = INTERACTION_RANGE * 2;
		torus.transform.localScale = torusScale;
		torus.SetActive(isActive);
	}
    private void Update()
    {
        if (interactingSystemInUse) { InteractingUpdate(); }
    }

	public void ActionStarted()
	{
		interactingSystemInUse = true;
		ResetVariables();
		PopulateListOfPotentialTargets();
		CreateDetectionTorus(true);
	}

	private void EndInteraction()
	{
		if (torus) { Destroy(torus); }
		interactingSystemInUse = false;
		turnManager.ActiveCharacter.actionManager.ReportEndOfAction();
        
	}

	public void Undo()
	{
		if (targetLockisHeld) { targetLockisHeld = false; }
		else { ActionCancel(); }
	}

	public void ActionCancel()
	{
		ResetVariables();
		EndInteraction();
	}

	public void Interact()
	{
		ActionComplete();
		if (targetLocked) 
		{
			IInteractable interactable = targetLocked.gameObject.GetComponentInChildren<IInteractable>();
			interactable.Interact();
		}
	}

	private void ActionComplete()
	{
		ResetVariables();
		EndInteraction();
	}

	public void ActionContinue()
	{
		Interact();
		ActionComplete();
	}


	private void InteractingUpdate()
	{
		if (torus && torus.activeSelf) { torus.transform.position = selfTarget.TargetPos; }
		if (!targetLockisHeld)
		{
			if (!targetLocked)
			{
				mouseLine.enabled = false;
			}
			PopulateListOfPotentialTargets();
			RestrictListOfTargetsToInteractionRange();
			RestrictListOfTargetsToLOS();
			UpdateColorOfTargetingIndicatorsBasedOnRangeAndLOS();
			TargetedPointFollowsMouse();
			Targetable prevClosestTargetNearMouse = closestTargetNearMouse;
			closestTargetNearMouse = DetermineClosestTargetNearMouse();
			if (closestTargetNearMouse)
			{
				targetLocked = DetermineIfClosestTargetIsLocked();
				if (targetLocked) { targetLocked.Select(); }
			}
			else { targetLocked = null; }
			SetTargetedPoint(prevClosestTargetNearMouse);
			DetermineImpactPoint();
			DetermineIfSightCanPenetrateImpactPoint();
		}
		DrawLineToTargetedPoint();
		if (!rotatingNow) { RotateTowardTargetedPoint(); }
		TargetedPointFollowsMouse();
		
		if (targetLocked && Vector3.Distance(mousePoint, targetLocked.TargetPos) < 10f)
		{
			if (!interacting && Input.GetMouseButtonDown(0) && targetLocked && !targetLockisHeld) 
			{ 
				targetLockisHeld = true;
				targetLocked.HoldSelection();
			}
			else if (!interacting && Input.GetMouseButtonDown(0) && targetLocked && targetLockisHeld) 
			{
				interacting = true;
				Interact(); 
				targetLocked.SelectionClear(); 
			}
		}
		if (Input.GetMouseButtonDown(1) && targetLockisHeld) 
		{ 
			targetLockisHeld = false;
			targetLocked.Select();
		}
		
	}

	private void SetTargetedPoint(Targetable prevClosestTargetNearMouse)
	{
		if (targetLocked)
		{
			targetedPoint = closestTargetNearMouse.TargetPos;
			targetLocked.Select();
			if (prevClosestTargetNearMouse && targetLocked != prevClosestTargetNearMouse)
			{
				prevClosestTargetNearMouse.SelectionClear();
			}
		}
		else
		{
			targetedPoint = mousePoint;
			if (prevClosestTargetNearMouse) 
			{ 
				prevClosestTargetNearMouse.SelectionClear();
				AgentLocalUI localUI = prevClosestTargetNearMouse.gameObject.GetComponent<AgentLocalUI>();
				if (localUI) { localUI.Reset(); }
			}
			if (closestTargetNearMouse) 
			{ 
				closestTargetNearMouse.SelectionClear();
				AgentLocalUI localUI = closestTargetNearMouse.gameObject.GetComponent<AgentLocalUI>();
				if (localUI) { localUI.Reset(); }
			}
		}
	}

	private Targetable DetermineClosestTargetNearMouse()
	{
		closestTargetNearMouse = null;
		float closestDistanceFromMouse = Mathf.Infinity;
		foreach (Targetable target in targetList)
		{
			float distFromMouse = Vector3.Distance(mousePoint, target.TargetPos);
			if (distFromMouse < closestDistanceFromMouse) 
			{ 
				closestDistanceFromMouse = distFromMouse; 
				closestTargetNearMouse = target; 
			}
		}
		return closestTargetNearMouse;
	}

	private Targetable DetermineIfClosestTargetIsLocked()
	{
		float distMouseToClosestTarget = Vector3.Distance(closestTargetNearMouse.TargetPos, mousePoint);
		float stickiness = SNAPPING_DISTANCE;
		if (targetLocked) { stickiness = stickiness * 1; }
		bool notTooFar = distMouseToClosestTarget < (1f+stickiness);
		bool farEnough = distMouseToClosestTarget > (1f-stickiness);
		if (farEnough && notTooFar) 
		{ 
			//TODO: and play sound effect?
			return closestTargetNearMouse; 
		}	
		else return null;
	}

	private void TargetedPointFollowsMouse()
	{
		Ray rayFromScreen = cameraForRaycastingToMouse.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouseInfo;
		int layerMask = LayerMask.GetMask("Floor");
		Physics.Raycast(rayFromScreen, out mouseInfo, 1000f, layerMask);
		mousePoint = mouseInfo.point;
		mousePoint.y = selfTarget.TargetPos.y;
		float distanceToMouse = Vector3.Distance(mousePoint, selfTarget.TargetPos);
		float clampedDistance = Mathf.Clamp(distanceToMouse, 0f, INTERACTION_RANGE);
		Vector3 vecTowardMouse = (mousePoint - selfTarget.TargetPos).normalized * clampedDistance;
		mousePoint = selfTarget.TargetPos + vecTowardMouse;


		if (!targetLocked) 
		{ 
			targetedPoint = mousePoint; 
			targetedPoint.y = selfTarget.TargetPos.y; 
		}
	}

	
	private void DetermineImpactPoint()
	{
		Vector3 origin = selfTarget.TargetPos;
		Vector3 dest = targetedPoint;
		Vector3 dir = (dest - origin).normalized;
		float maxDist = Vector3.Distance(origin, dest);
		RaycastHit hitInfo;
		bool impact = Physics.Raycast(origin, dir, out hitInfo, maxDist);
		if (targetLocked) 
		{
			Collider[] targetColliders = targetLocked.gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider targetCollider in targetColliders)
			{
				if (hitInfo.collider == targetCollider)
				{
					impact = false;
				}
			}
		}
		if (impact) { impactPoint = hitInfo.point; }
		else { impactPoint = targetedPoint; }
	}

	private void DetermineIfSightCanPenetrateImpactPoint()
	{
		Vector3 origin = selfTarget.TargetPos;
		Vector3 dest = targetedPoint;
		Vector3 dir = (dest - origin).normalized;
		float maxDist = Vector3.Distance(origin, dest);
		RaycastHit hitInfo;
		bool visionBlocked = Physics.Raycast(origin, dir, out hitInfo, maxDist);
		if (targetLocked)
		{
			Collider[] targetColliders = targetLocked.gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider targetCollider in targetColliders)
			{
				if (hitInfo.collider == targetCollider)
				{
					visionBlocked = false;
				}
			}
		}
		if (visionBlocked) { targetedPoint = impactPoint; }
	}

	private void DrawLineToTargetedPoint()
	{
		DrawLine(mouseLine, selfTarget.TargetPos, impactPoint);
	}

	private void DrawLine(LineRenderer rend, Vector3 start, Vector3 end)
	{
		rend.enabled = true;
		Vector2 lineEndpointOnScreen = cameraForRaycastingToMouse.WorldToScreenPoint(end);
		Ray ray = cameraForRaycastingToMouse.ScreenPointToRay(lineEndpointOnScreen);
		RaycastHit lineHitInfo;
		Physics.Raycast(ray, out lineHitInfo, 1000f);
		rend.startWidth = 0.05f;
		rend.endWidth = 0.05f;
		rend.positionCount = 2;
		Vector3[] positions = new Vector3[2];
		positions[0] = start;
		positions[1] = end;
		rend.SetPositions(positions);
		rend.useWorldSpace = true;
		rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	}

	private void RotateTowardTargetedPoint()
	{
		Vector3 forward = transform.forward;
		Vector3 target = targetedPoint - transform.position;
		forward.y = 0;
		target.y = 0;
		if (Vector3.Angle(forward, target) > FIELD_OF_VIEW)
		{
			rotatingNow = true;
			StartCoroutine(BeginRotatingTowardTargetedPoint(transform));
		}
		
	}

	private IEnumerator BeginRotatingTowardTargetedPoint(Transform character)
	{
		float angSpeed = 220f;
		float counter = 0f;
		Vector3 end = targetedPoint - transform.position;
		Vector3 start = transform.forward;
		end.y = 0f;
		start.y = 0f;
		float arcDist = Vector3.Angle(start, end);
		float duration = arcDist / angSpeed;
		float look = 0f;
		while (counter <= duration)
		{
			float dot = Vector3.Dot(end, transform.right);
			if (dot < 0) { dot = -1f; }
			else { dot = 1f; }
			look = 0f;
			counter = counter + Time.deltaTime;
			look += angSpeed * Time.deltaTime * dot;
			Vector3 rot = character.rotation.eulerAngles + new Vector3(0f, look, 0f);
			character.rotation = Quaternion.Euler(rot);
			yield return null;
		}
		rotatingNow = false;
	}



	public void ResetVariables()		////////////////////
	{
		targetLockisHeld = false;
		interacting = false;
		mouseLine.enabled = false;
		mousePoint = Vector3.zero;
		impactPoint = Vector3.zero;
		targetedPoint = Vector3.zero;
		targetList.Clear();
		ResetAllTargetsEverywhere();
	}

	private void PopulateListOfPotentialTargets()
	{
		Targetable[] allTargets = FindObjectsOfType<Targetable>();
		Targetable selfTarget = GetComponent<Targetable>();
		foreach (Targetable target in allTargets)
		{
			if (target.Interactable==true && target != selfTarget) 
			{ 
				targetList.Add(target);
			}
			else { target.HideTarget(); }
		}
	}

	private void RestrictListOfTargetsToInteractionRange()
	{
		List<Targetable> allInteractableTargets = new List<Targetable>();
		foreach (Targetable target in targetList)
		{
			allInteractableTargets.Add(target);
		}
		foreach (Targetable target in allInteractableTargets)
		{
			float range = Vector3.Distance(target.TargetPos, selfTarget.TargetPos);
			target.distance = range;
			if (range > INTERACTION_RANGE)
			{
				targetList.Remove(target);
				target.HideTarget();
			}
		}
	}

	private void RestrictListOfTargetsToLOS()
	{
		List<Targetable> allInteractableTargetsWithinInteractionRange = new List<Targetable>(); 
		foreach (Targetable targetFromList in targetList)
		{
			allInteractableTargetsWithinInteractionRange.Add(targetFromList);
		}
		foreach (Targetable target in allInteractableTargetsWithinInteractionRange)
		{
			Vector3 origin = selfTarget.TargetPos;
			Vector3 dest = target.TargetPos;
			Vector3 direction = (dest - origin).normalized;
			float maxDist = target.distance;
			RaycastHit hitInfo;
			Ray ray = new Ray(origin, direction);
			bool seeAnything = Physics.Raycast(ray, out hitInfo, maxDist);
			bool LOStoTarget = false;
			if (seeAnything)
			{
				Collider[] targetColliders = target.gameObject.GetComponentsInChildren<Collider>();
				foreach (Collider targetCollider in targetColliders)
				{
					if (hitInfo.collider == targetCollider)
					{
						LOStoTarget = true;
					}
				}
			}

			if (!LOStoTarget || !seeAnything)
			{
				targetList.Remove(target);
				target.HideTarget();
				target.lineOfSight = Targetable.LOS.Blocked;
			}
			else { target.lineOfSight = Targetable.LOS.Clear; }
		}
	}

	private void ResetAllTargetsEverywhere()
	{
		Targetable[] allTargets = FindObjectsOfType<Targetable>();
		foreach (Targetable target in allTargets)
		{
			target.SelectionClear();
			target.SetColor();
			target.HideTarget();
		}
	}

	private void UpdateColorOfTargetingIndicatorsBasedOnRangeAndLOS()
	{
		foreach (Targetable target in targetList)
		{
			Vector3 targetPos = target.TargetPos;
			float distanceToTarget = Vector3.Distance(selfTarget.TargetPos, targetPos);
			target.ShowTarget();
			target.SetColor(clearLOSColor);
		}
	}


}
