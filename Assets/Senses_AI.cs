using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AlertStatus { None, OnPatrol, OnAlert}

public class Senses_AI : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private GameObject alertIndicator;
	private Targetable player;
	private Targetable agent;
	[Header("For feedback only - do not modify in Unity")]
	[SerializeField] private AlertStatus alertStatus = AlertStatus.OnPatrol;
	private float detectionRange = 15f;
	[SerializeField] private float distanceToPlayer;
	[SerializeField] private bool playerInSight;
	[SerializeField] private bool playerInRange;
	private const float COOLDOWN_TIMER_MAX = 5f;
	private float cooldownTimer;
	private bool cooldownTimerOn = false;
	private Collider[] selfColliders;

	private void Start()
	{
		alertIndicator.SetActive(false);
		agent = GetComponent<Targetable>();
		cooldownTimer = COOLDOWN_TIMER_MAX;
		player = FindObjectOfType<PlayerHotkeyInput>().gameObject.GetComponent<Targetable>();
		selfColliders = GetComponentsInChildren<Collider>();
		AgentStats agentStats = GetComponent<AgentStats>();
		if (!agentStats.isHuman) { detectionRange = detectionRange * 2f; }
	}

	void Update()
	{
		playerInSight = RaycastTowardPlayer();
		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
		playerInRange = (distanceToPlayer < detectionRange);
		
		if (playerInSight && playerInRange) 
		{
			Alert(true);
            UmbraEventManager.instance.ActivateAlarm();
		}
		else 
		{
			Alert(false);
		}

		if (cooldownTimerOn && cooldownTimer < 0f)
		{
			cooldownTimerOn = false;
			alertStatus = AlertStatus.OnPatrol;
		}

		if (cooldownTimerOn) { cooldownTimer = cooldownTimer - Time.deltaTime; }
	}

	private void Alert (bool onAlert)
	{
		if (onAlert) 
		{
			alertIndicator.SetActive(true);
			alertStatus = AlertStatus.OnAlert;
			cooldownTimerOn = false;
			cooldownTimer = COOLDOWN_TIMER_MAX;
		}
		else 
		{
			if (cooldownTimer > 0f)
			{
				alertIndicator.SetActive(false);
				cooldownTimerOn = true;
			}
		}
	}

	private bool RaycastTowardPlayer()
	{
		float height = 1f;
		Vector3 originPoint = agent.TargetPos + (Vector3.up * height);
		Vector3 destPoint = player.TargetPos + (Vector3.up * height);
		Vector3 dir = (destPoint - originPoint).normalized;
		float maxDist = Vector3.Distance(originPoint, destPoint);
		int playerLayer = LayerMask.GetMask("Player");
		RaycastHit[] objectsHit = Physics.RaycastAll(originPoint, dir, maxDist);
		List<Collider> collidersHit = new List<Collider>();
		foreach (RaycastHit hit in objectsHit)
		{
			bool valid = true;
			foreach (Collider selfCollider in selfColliders)
			{
				if (selfCollider == hit.collider)
				{
					valid = false;
				}
				if (valid) { collidersHit.Add(hit.collider); }
			}
		}
		if (collidersHit.Count == 1 && collidersHit[0].gameObject.tag == "Player")
		{ 
			return true; 
		}
		else 
		{ 
			return false; 
		}
	}

	public AlertStatus GetAlertStatus()
	{
		return alertStatus;
	}
}
