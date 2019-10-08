using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentShooting : MonoBehaviour
{
	[SerializeField] Transform firePoint;
	[SerializeField] Material lineMaterial;
	[SerializeField] Material lineMaterialBlocked;
	private bool shootingSystemInUse = false;
	public bool ShootingSystemInUse { get { return shootingSystemInUse; } }
    public Animator animator;

	private AgentTurnManager turnManager;
	private List<Targetable> targetList;
	private Targetable closestTargetNearMouse;
	private Targetable targetLocked;
	private WeaponDesign weapon;
	private enum ShootingMode { Aiming, Firing, None}
	private ShootingMode shootingMode = ShootingMode.None;
	private Color partialLOSColor = Color.yellow;
	private Color defaultColor = Color.white;
	private Color clearLOSColor = Color.green;
	private Color noLOSColor = Color.red;
	private float optimumRange = 0;
	private float longRange = 0;
	private float maxRange = 0;
	private Stat damageBonus;
	private Vector3 targetedPoint;
	private Vector3 mousePoint;
	private Vector3 impactPoint;
	private Camera cameraForRaycastingToMouse;
	private LineRenderer mouseLineFree;
	private LineRenderer mouseLineBlocked;
	private enum EffectiveRange { Optimum, Long, Exceeded}
	private EffectiveRange rangeCat;
	private const float FIELD_OF_VIEW = 5f;
	private const float MAX_ENCOUNTER_RANGE = 50f;
	private const float MAX_AIMING_RADIUS = 0.25f;
	private const float SNAPPING_DISTANCE = 1.5f;
	private bool rotatingNow = false;
	private Collider[] selfColliders;
	Targetable selfTarget;
	private bool targetLockisHeld = false;
	private float shotsToResolve = Mathf.Infinity;
	private int accuracy = 0;

	private void Start()
    {
		turnManager = FindObjectOfType<AgentTurnManager>();
		selfTarget = gameObject.GetComponent<Targetable>();
		targetList = new List<Targetable>();
		cameraForRaycastingToMouse = Camera.main;
		selfColliders = this.gameObject.GetComponentsInChildren<Collider>();
		GameObject mouseLineGO = new GameObject();
		GameObject mouseLine2GO = new GameObject();
		mouseLineFree = mouseLineGO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		mouseLineBlocked = mouseLine2GO.AddComponent(typeof(LineRenderer)) as LineRenderer;
		mouseLineFree.material = lineMaterial;
		mouseLineBlocked.material = lineMaterialBlocked;
		mouseLineFree.enabled = false;
		mouseLineBlocked.enabled = false;

	}

    private void Update()
    {
        if (shootingMode==ShootingMode.Aiming) { AimingUpdate(); }
    }

	public void ActionStarted()
	{
		shootingSystemInUse = true;
		ResetVariables();
		AgentStats stats = GetComponent<AgentStats>();
		if (stats)
		{
			weapon = stats.EquippedWeapon;
			damageBonus = stats.damageBonus;
		}
		PopulateListOfPotentialTargets();
		BeginAiming();
	}

	public void ActionContinue()
	{
		if (targetLocked)
		{
			BeginFiring();
		}
	}

	public void ActionCancel()
	{
		animator.SetBool("isPistolDrawn", false);
		ResetVariables();
		EndShooting();
	}

	public void ActionComplete()
	{
		ResetVariables();
		EndShooting();
	}

	public void Undo()
	{
		if (targetLockisHeld) { targetLockisHeld = false; }
		else { ActionCancel(); }
	}

	private void EndShooting()
	{
		closestTargetNearMouse.SelectionClear();
		ShowAccuracy(0, closestTargetNearMouse, false);
		animator.SetBool("isPistolDrawn", false);
		shootingSystemInUse = false;
		turnManager.ActiveCharacter.actionManager.ReportEndOfAction();
	}

	private void BeginAiming()
	{
		shootingMode = ShootingMode.Aiming;
		UpdateTargetsWithWeaponRangeCategoryInfo();
		UpdateColorOfTargetingIndicatorsBasedOnRangeAndLOS();
        animator.SetBool("isPistolDrawn", true);
	}

	private void BeginFiring()
	{
		shootingMode = ShootingMode.Firing;
		mouseLineFree.enabled = false;
		mouseLineBlocked.enabled = false;
		ShowAccuracy(0, targetLocked, false);
		StartCoroutine("ShootProjectiles");
		StartCoroutine("DelayedFinish");
	}



	private void AimingUpdate()
	{
		if (!targetLockisHeld)
		{
			if (!targetLocked)
			{
				mouseLineFree.enabled = false;
				mouseLineBlocked.enabled = false;
			}
			PopulateListOfPotentialTargets();
			RestrictListOfTargetsToEncounterRange();
			UpdateTargetsWithDistanceInfo();
			RestrictListOfTargetsToLOS();
			UpdateTargetsWithCoverStatus();
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
			accuracy = 0;
			bool targetHasCover = false;
			List<RaycastHit> colliderHitList = new List<RaycastHit>();
			if (targetLocked)
			{
				colliderHitList = DetermineIfTargetHasCover();
				if (colliderHitList.Count > 0) { targetHasCover = true; }
				accuracy = Mathf.Clamp(DetermineAccuracy(targetHasCover), 0, 98);
				ShowAccuracy(accuracy, targetLocked, true);
			}
			SetTargetedPoint(prevClosestTargetNearMouse);
			DetermineImpactPoint();
			DetermineIfSightCanPenetrateImpactPoint();
		}
		DrawLineToTargetedPoint();
		if (!rotatingNow) { RotateTowardTargetedPoint(); }
		TargetedPointFollowsMouse();
		if (targetLocked && Vector3.Distance(mousePoint, targetLocked.TargetPos) < 2f)
		{
			if (shootingMode == ShootingMode.Aiming) { 
				if (Input.GetMouseButtonDown(0) && targetLocked && !targetLockisHeld) { targetLockisHeld = true; }
				else if (Input.GetMouseButtonDown(0) && targetLocked && targetLockisHeld) { BeginFiring(); }
				if (Input.GetMouseButtonDown(1) && targetLockisHeld) { targetLockisHeld = false; }
				else if (Input.GetMouseButtonDown(1)) { ActionCancel(); }
			}
		}
	}

	void ShootProjectile()
	{
		GameObject projectileGO = Instantiate(weapon.projectilePrefab, firePoint.position, Quaternion.LookRotation(transform.forward));
		projectileGO.transform.LookAt(targetLocked.transform);
		Projectile projectile = projectileGO.GetComponent<Projectile>();
		projectile.SetShooter(selfColliders);
		projectile.SetTarget(targetLocked);
		projectile.SetWeapon(weapon);
		projectile.HitTarget(DetermineHit());
		projectile.SetDamageBonus(damageBonus.GetValue());
	}

	private bool DetermineHit()
	{
		int rollToHit = Random.Range(1, 100);
		bool hit = false;
		if (rollToHit<=accuracy) { hit = true; }
		return hit;
	}

	IEnumerator ShootProjectiles()
	{
		shotsToResolve = 1;
		float shooting_delay = Random.Range(0.2f,0.5f);
		float counter = 0f;
		for (int i = 0; i < shotsToResolve; i++)
		{
			ShootProjectile();
			while (counter < shooting_delay)
			{
				counter += Time.deltaTime;
				yield return null;
			}
		}
	}

	IEnumerator DelayedFinish()
	{
		float delay = 1f;
		float counter = 0f;
		while (counter<delay)
		{
			counter += Time.deltaTime;
			yield return null;
		}
		ActionComplete();
	}



	private void UpdateTargetsWithWeaponRangeCategoryInfo()
	{
		int shootingRange = (int)weapon.range;
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

	private List<RaycastHit> DetermineIfTargetHasCover()
	{
		bool hasCover = false;
		Vector3 origin = firePoint.position;
		Vector3 direction = targetLocked.TargetPos - firePoint.position;
		float maxDist = Vector3.Distance(firePoint.position, targetLocked.TargetPos);
		float radius = 0.25f;
		RaycastHit[] hitsArray;
		hitsArray = Physics.SphereCastAll(origin, radius, direction, maxDist);
		List<RaycastHit> hitList = new List<RaycastHit>();
		List<Collider> collidersToIgnore = new List<Collider>();
		Collider[] targetColliders = targetLocked.gameObject.GetComponentsInChildren<Collider>();
		foreach (Collider collider in selfColliders) { collidersToIgnore.Add(collider); }
		foreach (Collider collider in targetColliders) { collidersToIgnore.Add(collider); }
		foreach (RaycastHit hit in hitsArray)
		{
			bool valid = true;
			foreach (Collider collider in collidersToIgnore) 
			{
				if (hit.collider==collider) { valid = false; }
			}
			if (valid) { hitList.Add(hit); }
		}
		if (hitList.Count > 0) { hasCover = true; }
		return hitList;
	}

	private int DetermineAccuracy(bool hasCover)
	{
		AgentStats stats = GetComponent<AgentStats>();
		AgentStats enemyStats = targetLocked.gameObject.GetComponent<AgentStats>();
		int shootSkill = Mathf.Clamp(stats.Shooting.GetValue(),0,6);
		int baseAcc = 40;
		int rangeBonus = 0;
		if (weapon.weaponType == ItemType.Rifle) { rangeBonus += 5; }
		else if (weapon.weaponType == ItemType.Pistol
				&& targetLocked.rangeToTarget == Targetable.RangeCat.Optimum) { rangeBonus += 10; }
		int weaponAcc = (int)weapon.accuracy;
		int acc = baseAcc + rangeBonus + stats.accuracyBonus.GetValue();
		if (hasCover) 
		{
			int maxCover = 40 + enemyStats.coverBonus.GetValue(); 
			int coverPenalty = maxCover - stats.coverBypass.GetValue() - weaponAcc;
			if (enemyStats) { coverPenalty = coverPenalty + enemyStats.coverBonus.GetValue(); }
			coverPenalty = Mathf.Clamp(coverPenalty, 0, maxCover);
			acc = acc - coverPenalty;
			AgentLocalUI targetUI = targetLocked.gameObject.GetComponent<AgentLocalUI>();
			if (targetUI) { targetUI.ShowCoverNotice(coverPenalty); }
		}
		return acc;
	}

	private void ShowAccuracy(int accuracy, Targetable target, bool toShow)
	{

        if (target == null) { return; }
		AgentLocalUI targetAgentUI = target.gameObject.GetComponent<AgentLocalUI>();
		if (!targetAgentUI) { return; }
		if (toShow) 
		{ 
			targetAgentUI.ShowAccuracy(accuracy); 
		} else 
		{ 
			targetAgentUI.Reset(); 
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
				ShowAccuracy(0, prevClosestTargetNearMouse, false);
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
		//float DistTargetToPlayer = Vector3.Distance(closestTargetNearMouse.TargetPos, firePoint.position);
		//float DistMouseToPlayer = Vector3.Distance(mousePoint, firePoint.position);
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

		if (!targetLocked) 
		{ 
			targetedPoint = mousePoint; 
			targetedPoint.y = selfTarget.TargetPos.y; 
		}
	}

	
	private void DetermineImpactPoint()
	{
		Vector3 origin = firePoint.position;
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
		Vector3 height = Vector2.up * MAX_AIMING_RADIUS;
		Vector3 origin = firePoint.position + height;
		Vector3 dest = targetedPoint + height;
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
		/*Vector3 shortestDistHit = targetedPoint;
		float shortestDist = Vector3.Distance(firePoint.position, shortestDistHit);
		foreach (RaycastHit hit in hitList)
		{
			float distToHit = Vector3.Distance(firePoint.position, hit.point);
			if (distToHit < shortestDist)
			{
				shortestDistHit = hit.point;
			}
		}
		Vector3 impactPoint = shortestDistHit;
		*/
		DrawLine(mouseLineFree, firePoint.position, impactPoint);
		if (Vector3.Distance(impactPoint, targetedPoint) > 0.1f)
		{
			DrawLine(mouseLineBlocked, impactPoint, targetedPoint);
		}
		else { mouseLineBlocked.enabled = false; }
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
		mouseLineBlocked.enabled = false;
		mouseLineFree.enabled = false;
		mousePoint = Vector3.zero;
		impactPoint = Vector3.zero;
		targetedPoint = Vector3.zero;
		shootingMode = ShootingMode.None;
		optimumRange = 0f;
		longRange = 0f;
		maxRange = 0f;
		accuracy = 0;
		targetList.Clear();
		ResetAllTargetsEverywhere();
		shotsToResolve = Mathf.Infinity;
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

	private void RestrictListOfTargetsToEncounterRange()
	{
		List<Targetable> allShootableTargets = targetList;
        List<Targetable> toRemove = new List<Targetable>();
		foreach (Targetable target in allShootableTargets)
		{
			float range = Vector3.Distance(target.TargetPos, selfTarget.TargetPos);
			target.distance = range;
			if (range > MAX_ENCOUNTER_RANGE)
			{
                //targetList.Remove(target);
                toRemove.Add(target);
				target.HideTarget();
			}
		}

        foreach (Targetable target in toRemove)
        {
            allShootableTargets.Remove(target);
        }
	}

	private void RestrictListOfTargetsToLOS()
	{
		List<Targetable> allShootableTargetsWithinEncounterRange = new List<Targetable>(); 
		foreach (Targetable targetFromList in targetList)
		{
			allShootableTargetsWithinEncounterRange.Add(targetFromList);
		}
		Vector3 height = (Vector3.up * MAX_AIMING_RADIUS);
		foreach (Targetable target in allShootableTargetsWithinEncounterRange)
		{
			Vector3 origin = selfTarget.TargetPos + height;
			Vector3 dest = target.TargetPos + height;
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

	private void UpdateTargetsWithCoverStatus()
	{
		List<Targetable> allShootableUnblockedTargetsWithinEncounterRange = new List<Targetable>();
		foreach (Targetable targetFromList in targetList)
		{
			allShootableUnblockedTargetsWithinEncounterRange.Add(targetFromList);
		}
		Vector3 height = (Vector3.up * MAX_AIMING_RADIUS);
		foreach (Targetable target in allShootableUnblockedTargetsWithinEncounterRange)
		{
			Vector3 origin = selfTarget.TargetPos + (height * -1);
			Vector3 dest = target.TargetPos + (height * -1);
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
				target.lineOfSight = Targetable.LOS.Cover;
			}
			else 
			{
				target.lineOfSight = Targetable.LOS.Clear; 
			}
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

	private void UpdateTargetsWithDistanceInfo()
	{
		foreach (Targetable target in targetList)
		{
			target.distance = Vector3.Distance(target.TargetPos, selfTarget.TargetPos);
		}
	}
	

	private void UpdateColorOfTargetingIndicatorsBasedOnRangeAndLOS()
	{
		foreach (Targetable target in targetList)
		{
			Vector3 targetPos = target.TargetPos;
			float distanceToTarget = Vector3.Distance(firePoint.position, targetPos);
			target.ShowTarget();
			if (target.rangeToTarget==Targetable.RangeCat.Optimum
				&& target.lineOfSight==Targetable.LOS.Clear) 
			{ 
				target.SetColor(clearLOSColor); 
			}
			else if (target.rangeToTarget==Targetable.RangeCat.Long
					|| target.lineOfSight==Targetable.LOS.Cover) 
			{ 
				target.SetColor(partialLOSColor); 
			}
			else { target.SetColor(noLOSColor); }
		}
	}
}
