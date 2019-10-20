using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitStatus { None, Hit, Miss}
public class Projectile : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private float speed = 5f;
	[SerializeField] private GameObject ExplosionPrefab;
	private GameObject explosionEffect;
	private float maxDistance = 50f;
	private float distance = 0f;
	private Vector3 direction = Vector3.zero;
	private HitStatus hit = HitStatus.None;
	private int damage = 0;
	private WeaponDesign weapon;
	private Targetable target;
	private Collider[] shooterColliders;
	private int damageBonus;
	private Shooting_AI shooting_AI = null;
	private bool endTurnReported=false;
	private const float ARRIVAL_THRESHOLD = 0.1f;

	void Update()
    {
		distance = distance + (speed * Time.deltaTime);
		Vector3 vec = direction * (speed * Time.deltaTime);
		transform.position = transform.position + vec;
		if (distance >= maxDistance) { Destroy(this.gameObject); }

		float distanceToTarget = Vector3.Distance(transform.position, target.TargetPos);
		if (shooting_AI && !endTurnReported && distanceToTarget<ARRIVAL_THRESHOLD) { ReportEndTurn(); }
	}

	private void OnTriggerEnter(Collider collision)
	{
		bool selfHit = false;
		foreach (Collider shooterCollider in shooterColliders)
		{
			if (collision==shooterCollider) { selfHit = true; }
		}

		if (selfHit==false &&
			(collision.gameObject.layer == LayerMask.NameToLayer("Agent") ||
			collision.gameObject.layer == LayerMask.NameToLayer("NPC") ||
			collision.gameObject.layer == LayerMask.NameToLayer("Player")))
		{
			CombatReactions react = collision.gameObject.GetComponent<CombatReactions>();
			if (react) { react.TakeHit(hit, damage); }
			if (hit == HitStatus.Hit)
			{
				explosionEffect = Instantiate(ExplosionPrefab, collision.ClosestPoint(transform.position), Quaternion.identity);
				Destroy(explosionEffect, 1f);
			}
			Destroy(this.gameObject,0.01f);
		}
	}

	public void SetTarget (Targetable _target)
	{
		target = _target;
		direction = (target.TargetPos - transform.position).normalized;
	}
	public void SetWeapon (WeaponDesign _weapon)
	{
		weapon = _weapon;
	}

	public void SetDamageBonus (int bonus)
	{
		damageBonus = bonus;
	}

	public void SetShooting_AI (Shooting_AI shootingAI)
	{
		this.shooting_AI = shootingAI;	
	}

	public void HitTarget(bool targetIsHit)
	{
		if (targetIsHit == true) { hit = HitStatus.Hit; }
		else { hit = HitStatus.Miss; }
		if (hit == HitStatus.Hit) 
		{
			damage = (int)weapon.damage;
			damage = damage + damageBonus;
			damage = Mathf.RoundToInt(damage * Random.Range(0.5f, 1.5f));
		}
	}

	public void SetShooter (Collider[] _shooter)
	{
		shooterColliders = _shooter;
	}

	private void OnDestroy()
	{
		if (shooting_AI && !endTurnReported)
		{
			ReportEndTurn();
		}
	}

	private void ReportEndTurn()
	{
		if (!endTurnReported && shooting_AI)
		{
			endTurnReported = true;
			shooting_AI.ReportEndTurn();
		}
	}

}
