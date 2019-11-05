using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting_AI : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private Animator animator;
	private AgentActionManager actionManager;
	private Targetable player;
	private Targetable selfTarget;
	private Collider[] selfColliders;
	private AgentStats self;
	private Transform firePoint;
	private Senses_AI senses;
	private const float FIELD_OF_VIEW = 5f;
	private const float ROTATION_THRESHOLD = 1f;
	private const float WEAPON_DRAW_THRESHOLD = 35f;
	private const float MAX_AIMING_RADIUS = 0.25f;
	private const float END_TURN_DELAY = 0.75f;
	private bool rotatingNow = false;
	bool shootingSystemInUse = false;
	bool projectileHasBeenShot = false;
	private enum Activity { HoldingFire, Shooting, None}
	private Activity activity = Activity.None;

	private void Start()
	{
		self = GetComponent<AgentStats>();
		selfTarget = GetComponent<Targetable>();
		selfColliders = GetComponentsInChildren<Collider>();
		actionManager = GetComponent<AgentActionManager>();
		firePoint = GetComponentInChildren<FirePoint>().transform;
		senses = GetComponent<Senses_AI>();
		player = FindObjectOfType<PlayerHotkeyInput>().gameObject.GetComponent<Targetable>();
	}

	private void Update()
	{
		if (shootingSystemInUse && !projectileHasBeenShot) { ShootUpdate(); }
	}

	private void ShootUpdate()
	{
		float angleTowardPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);
		if (!rotatingNow) { RotateTowardPlayer(); }
				
		if (angleTowardPlayer < WEAPON_DRAW_THRESHOLD)
		{
			//DrawWeapon(true);		Disable while bug fixing. Should try re-enabling when bugs have been fixed.
		}

		if (angleTowardPlayer < FIELD_OF_VIEW)
		{
			RangeCategory rangeToPlayer = DetermineRangeToPlayer();
			if (rangeToPlayer == RangeCategory.Optimum || rangeToPlayer == RangeCategory.Long)
			{
				bool playerIsInLOS = DetermineIfPlayerIsInLOS();
				if (!playerIsInLOS) { ActionComplete(); return; }
				bool playerHasCover = DetermineIfPlayerHasCover();
				float accuracy = DetermineAccuracy(playerHasCover, rangeToPlayer);
               
				ShootAtPlayer(accuracy);
				projectileHasBeenShot = true;
			}
			else { ActionComplete(); }
			
		}
	}

	public void ActionStarted()
	{
		shootingSystemInUse = true;
		projectileHasBeenShot = false;
		if (senses.GetAlertStatus() == AlertStatus.OnAlert) { activity = Activity.Shooting; }
		else { ActionComplete(); }
	}

	public void ActionComplete()
	{
		activity = Activity.None;
		DrawWeapon(false);
		shootingSystemInUse = false;
		projectileHasBeenShot = false;
		actionManager.ReportEndOfShooting_AI();
	}

	private void DrawWeapon(bool toDraw)
	{
		if (self.isHuman && animator)
		{
			AnimatorControllerParameter[] parameters = animator.parameters;
			foreach (AnimatorControllerParameter parameter in parameters)
			{
				if (parameter.name == "isPistolDrawn")
				{
					animator.SetBool("isPistolDrawn", toDraw);
				}
			}
		}
		
	}
	
	private void RotateTowardPlayer()
	{
		Vector3 forward = transform.forward;
		Vector3 target = player.TargetPos - selfTarget.TargetPos;
		forward.y = 0;
		target.y = 0;
		if (Vector3.Angle(forward, target) > ROTATION_THRESHOLD)
		{
			rotatingNow = true;
			StartCoroutine(BeginRotatingTowardTargetedPoint(transform));
		}
	}

	private IEnumerator BeginRotatingTowardTargetedPoint(Transform character)
	{
		float angSpeed = 220f;
		float counter = 0f;
		Vector3 end = player.TargetPos - selfTarget.TargetPos;
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

	
	private bool DetermineIfPlayerHasCover()
	{
		Vector3 origin = firePoint.position;
		Vector3 direction = player.TargetPos - firePoint.position;
		float maxDist = Vector3.Distance(firePoint.position, player.TargetPos);
		float radius = MAX_AIMING_RADIUS;
		RaycastHit[] hitsArray;
		hitsArray = Physics.SphereCastAll(origin, radius, direction, maxDist);
		List<RaycastHit> hitList = new List<RaycastHit>();
		List<Collider> collidersToIgnore = new List<Collider>();
		Collider[] playerColliders = player.gameObject.GetComponentsInChildren<Collider>();
		foreach (Collider collider in selfColliders) { collidersToIgnore.Add(collider); }
		foreach (Collider collider in playerColliders) { collidersToIgnore.Add(collider); }
		foreach (RaycastHit hit in hitsArray)
		{
			bool valid = true;
			foreach (Collider collider in collidersToIgnore)
			{
				if (hit.collider == collider) { valid = false; }
			}
			if (valid) { hitList.Add(hit); }
		}
		if (hitList.Count > 0) { return true; }
		else { return false; }
	}

	private bool DetermineIfPlayerIsInLOS()
	{
		//Vector3 height = (Vector3.up * MAX_AIMING_RADIUS);
		float height = firePoint.position.y;
		float allowanceForHighShot = 0.2f;
		Vector3 origin = selfTarget.TargetPos;
		origin.y = 0f + height;
		Vector3 dest = player.TargetPos;
		dest.y = 0f + height + allowanceForHighShot;
		Vector3 direction = (dest - origin).normalized;
		float maxDist = Vector3.Distance(player.TargetPos, selfTarget.TargetPos); ;
		RaycastHit hitInfo;
		Ray ray = new Ray(origin, direction);
		bool seeAnything = Physics.Raycast(ray, out hitInfo, maxDist);
		bool LOStoTarget = false;
		if (seeAnything)
		{
			Collider[] playerColliders = player.gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider playerCollider in playerColliders)
			{
					if (hitInfo.collider == playerCollider)
					{
						LOStoTarget = true;
					}
				}
			}

		if (!LOStoTarget || !seeAnything)
		{
			return false;
		}
		else { return true; }
	}

	private RangeCategory DetermineRangeToPlayer()
	{
		int maxWeaponRange = (int)self.EquippedWeapon.range;
		float maxRange = (float)maxWeaponRange;
		float optimumRange = maxRange / 2f;
		float longRange = maxRange - optimumRange;
		RangeCategory playerRange = RangeCategory.Exceeded;
		if (firePoint != null)
		{
			float distanceToTarget = Vector3.Distance(firePoint.position, player.TargetPos);
			if (distanceToTarget < optimumRange) { playerRange = RangeCategory.Optimum; }
			else if (distanceToTarget < maxRange) { playerRange = RangeCategory.Long; }
			else { playerRange = RangeCategory.Exceeded; }
		}
		return playerRange;
	}

	private float DetermineAccuracy(bool hasCover, RangeCategory rangeToPlayer)
	{
		AgentStats playerStats = player.gameObject.GetComponent<AgentStats>();
		int shootSkill = Mathf.Clamp(self.Shooting.GetValue(), 0, 6);
		int baseAcc = 30;
		int rangeBonus = 0;
		if (self.EquippedWeapon.weaponType == ItemType.Rifle) { rangeBonus += 5; }
		else if (self.EquippedWeapon.weaponType == ItemType.Pistol
				&& rangeToPlayer == RangeCategory.Optimum) { rangeBonus += 10; }
		int weaponAccModifier = (int)self.EquippedWeapon.accuracy;
		int acc = baseAcc + rangeBonus + self.accuracyBonus.GetValue();
		if (hasCover)
		{
			int maxCover = 40 + playerStats.coverBonus.GetValue();
			int coverPenalty = maxCover - (self.coverBypass.GetValue() - weaponAccModifier);
			if (playerStats) { coverPenalty = coverPenalty + playerStats.coverBonus.GetValue(); }
			coverPenalty = Mathf.Clamp(coverPenalty, 0, maxCover);
			acc = acc - coverPenalty;
		}
		return acc;
	}


	private void ShootAtPlayer(float acc)
	{
		float aimHeight = 1f;
		UmbraEventManager.instance.ActivateAlarm();
		GameObject projectileGO = Instantiate(self.EquippedWeapon.projectilePrefab, firePoint.position, Quaternion.LookRotation(transform.forward));

		if (self.isHuman) {

			FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.gunshotPistol1, firePoint.position);
		}
		else
		{
			FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.mechAttack, firePoint.position);
		}
		projectileGO.transform.LookAt(player.transform.position + (Vector3.up * aimHeight));
		Projectile projectile = projectileGO.GetComponent<Projectile>();
		float distanceToTarget = Vector3.Distance(firePoint.position, player.TargetPos);
		float projectileFlightTime = distanceToTarget / projectile.Speed;
		projectile.SetTarget(player);
		projectile.SetTimer(projectileFlightTime);
		object[] objectArray = new object[2];
		objectArray[0] = projectileFlightTime;
		objectArray[1] = acc;
		StartCoroutine("ResolveHitAfterShootingProjectile", objectArray);
	}

	private IEnumerator ResolveHitAfterShootingProjectile(object[] objects)
	{
		float delay = (float)objects[0];
		float acc = (float)objects[1];
		bool endTurnReported = false;
		yield return new WaitForSeconds(delay);
		CombatReactions react = player.gameObject.GetComponent<CombatReactions>();
		HitStatus hitStatus = HitStatus.None;
		bool attackHit = DetermineHit(acc);
		if (attackHit) { hitStatus = HitStatus.Hit; }
		else { hitStatus = HitStatus.Miss; }
		int damage = (int)self.EquippedWeapon.damage + self.damageBonus.GetValue();
		int rndDmgSpread = 3;
		damage = Random.Range(damage - rndDmgSpread, damage + rndDmgSpread + 1);
		if (react) { react.TakeHit(hitStatus, damage); }
		if (hitStatus == HitStatus.Hit)
		{
			GameObject explosionEffect = Instantiate(self.EquippedWeapon.projectilePrefab.GetComponent<Projectile>().Explosion, player.TargetPos, Quaternion.identity);
			Destroy(explosionEffect, 1f);
		}
		Shooting_AI shooting_AI = GetComponent<Shooting_AI>();
		if (shooting_AI && !endTurnReported)
		{
			endTurnReported = true;
			shooting_AI.ReportEndTurn();
		}
	}



	private bool DetermineHit(float accuracy)
	{
		int rollToHit = Random.Range(1, 100);
		float rollToHitAsFloat = (float)rollToHit;
		bool hit = false;
		if (accuracy==rollToHitAsFloat || rollToHitAsFloat < accuracy) { hit = true; }
		return (hit);
	}

	public void ReportEndTurn()
	{
		StartCoroutine("EndTurn");
	}

	private IEnumerator EndTurn()
	{
		yield return new WaitForSeconds(END_TURN_DELAY);
		ActionComplete();
	}
}
