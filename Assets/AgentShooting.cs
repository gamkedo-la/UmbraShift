using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentShooting : MonoBehaviour
{
	[SerializeField] Transform firePoint;
	[SerializeField] Material lineMaterial;
	private bool shootingSystemInUse = false;
	public bool ShootingSystemInUse { get { return shootingSystemInUse; } }
    public Animator animator;

	private List<Targetable> targetList;
	private Targetable closestTargetNearMouse;
	private Targetable targetLocked;
	private WeaponDesign weapon;
	private enum ShootingMode { Aiming, Firing, None}
	private ShootingMode shootingMode = ShootingMode.None;
	private Color outOfRangeColor = Color.yellow;
	private Color defaultColor = Color.white;
	private Color validColor = Color.green;
	private Color invalidColor = Color.red;
	private float optimumRange = 0;
	private float longRange = 0;
	private float maxRange = 0;
	private Vector3 targetedPoint;
	private Vector3 mousePoint;
	private Camera cameraForRaycastingToMouse;
	private LineRenderer mouseLine;
	private enum EffectiveRange { Optimum, Long, Exceeded}
	private EffectiveRange rangeCat;
	private const float FIELD_OF_VIEW = 5f;
	private bool rotatingNow = false;


	private void Start()
    {
		targetList = new List<Targetable>();
		cameraForRaycastingToMouse = Camera.main;
		GameObject mouseLineGO = new GameObject();
		mouseLine = mouseLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
        mouseLine.GetComponent<LineRenderer>().enabled = false;
        //animator = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        if (shootingMode==ShootingMode.Aiming) { AimingUpdate(); }
		else if (shootingMode==ShootingMode.Firing) { FiringUpdate(); }
    }

	public void ActionStarted()
	{
		shootingSystemInUse = true;
		ResetVariables();
		weapon = GetComponent<AgentStats>().EquippedWeapon;
		PopulateListOfPotentialTargets();
		BeginAiming();
	}

	private void BeginAiming()
	{
		shootingMode = ShootingMode.Aiming;
		DetermineRange();
		ActivateTargetingIndicators();
        animator.SetBool("isPistolDrawn", true);
       
	}

	private void AimingUpdate()
	{
		PopulateListOfPotentialTargets();
		TargetedPointFollowsMouse();
		DetermineRange();
		Targetable prevClosestTargetNearMouse = closestTargetNearMouse;
		closestTargetNearMouse = DetermineClosestTargetNearMouse();
		if (closestTargetNearMouse) { targetLocked = DetermineIfClosestTargetIsLocked(); }
		else { targetLocked = null; }
		SetTargetedPoint(prevClosestTargetNearMouse);
		if (targetLocked) { ShowAccuracy(targetLocked, true); }
		DrawLineToTargetedPoint();
		if (!rotatingNow) { RotateTowardTargetedPoint(); }
		//snap targetedpoint to target if mouse is close to target
		//change color of mouse range based on range
		//snap mouse to target position if close enough
		//prevent mouse from flying off screen
	}

	private void FiringUpdate()
	{



        animator.SetBool("isPistolDrawn", false);
	}

	private void DetermineRange()
	{
		int shootingRange = (int)weapon.Range;
		maxRange = (float)shootingRange;
		optimumRange = maxRange / 2f;
		longRange = maxRange - optimumRange;
		foreach (Targetable target in targetList)
		{
			Vector3 targetPos = target.TargetPos;
			float distanceToTarget = Vector3.Distance(firePoint.position, targetPos);
			if (distanceToTarget < optimumRange) { target.rangeToTarget = Targetable.RangeCat.Optimum; }
			else if (distanceToTarget < maxRange) { target.rangeToTarget = Targetable.RangeCat.Long; }
			else { target.rangeToTarget = Targetable.RangeCat.Exceeded; }
		}
	}

	private void ShowAccuracy(Targetable target, bool toShow)
	{
		AgentLocalUI targetAgentUI = target.gameObject.GetComponent<AgentLocalUI>();
		if (!targetAgentUI) { return; }
		if (toShow) { targetAgentUI.ShowAccuracy(); } else { targetAgentUI.Reset(); }
	}

	private void SetTargetedPoint(Targetable prevClosestTargetNearMouse)
	{
		if (targetLocked)
		{
			targetedPoint = closestTargetNearMouse.TargetPos;
			targetLocked.LockedOn = true;
			if (prevClosestTargetNearMouse && targetLocked != prevClosestTargetNearMouse)
			{
				prevClosestTargetNearMouse.LockedOn = false;
				ShowAccuracy(prevClosestTargetNearMouse, false);
			}
		}
		else
		{
			targetedPoint = mousePoint;
			if (prevClosestTargetNearMouse) 
			{ 
				prevClosestTargetNearMouse.LockedOn = false;
				AgentLocalUI localUI = prevClosestTargetNearMouse.gameObject.GetComponent<AgentLocalUI>();
				if (localUI) { localUI.Reset(); }
			}
			if (closestTargetNearMouse) 
			{ 
				closestTargetNearMouse.LockedOn = false;
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
		//float DistTargetToPlayer = Vector3.Distance(closestTargetNearMouse.TargetPos, firePoint.position);
		//float DistMouseToPlayer = Vector3.Distance(mousePoint, firePoint.position);
		float distMouseToClosestTarget = Vector3.Distance(closestTargetNearMouse.TargetPos, mousePoint);
		float stickiness = 1f;
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
		Ray ray = cameraForRaycastingToMouse.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo, 1000f);
		mousePoint = hitInfo.point;
		//mousePoint.y = firePoint.position.y;
		if (!targetLocked) { targetedPoint = mousePoint; }
	}

	private void DrawLineToTargetedPoint()
	{
		mouseLine.enabled = true;
		mouseLine.material = lineMaterial;
		lineMaterial.color = Color.white;
		Vector2 point = cameraForRaycastingToMouse.WorldToScreenPoint(targetedPoint);
		Ray ray = cameraForRaycastingToMouse.ScreenPointToRay(point);
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo, 1000f);
		mouseLine.startWidth = 0.05f;
		mouseLine.endWidth = 0.05f;
		mouseLine.positionCount = 2;
		Vector3[] positions = new Vector3[2];
		positions[0] = firePoint.position;
		positions[1] = targetedPoint;
		mouseLine.SetPositions(positions);
		mouseLine.useWorldSpace = true;
		mouseLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
		shootingMode = ShootingMode.None;
		optimumRange = 0f;
		longRange = 0f;
		maxRange = 0f;
		targetList.Clear();
		ResetAllTargetsEverywhere();
	}

	private void PopulateListOfPotentialTargets()
	{
		Targetable[] allTargets = FindObjectsOfType<Targetable>();
		Targetable selfTarget = GetComponent<Targetable>();
		foreach (Targetable target in allTargets)
		{
			if (target.Shootable==true && target != selfTarget) 
			{ 
				targetList.Add(target);
			}
			else { target.HideTarget(); }
		}
	}

	private void ResetAllTargetsEverywhere()
	{
		Targetable[] allTargets = FindObjectsOfType<Targetable>();
		foreach (Targetable target in allTargets)
		{
			target.LockedOn = false;
			target.SetColor();
			target.HideTarget();
		}
	}

	private void ActivateTargetingIndicators()
	{
		foreach (Targetable target in targetList)
		{
			Vector3 targetPos = target.TargetPos;
			float distanceToTarget = Vector3.Distance(firePoint.position, targetPos);
			target.ShowTarget();
			if (target.rangeToTarget==Targetable.RangeCat.Optimum) { target.SetColor(validColor); }
			else if (target.rangeToTarget==Targetable.RangeCat.Long) { target.SetColor(outOfRangeColor); }
			else { target.SetColor(invalidColor); }
		}
	}
}
