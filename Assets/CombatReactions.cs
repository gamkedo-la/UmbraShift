using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatReactions : MonoBehaviour
{

	private AgentLocalUI localUI;
	private AgentStats agentStats;
	private bool dying = false;
	private GameObject self;

	private void Start()
	{
		localUI = GetComponent<AgentLocalUI>();
		agentStats = GetComponent<AgentStats>();
		self = gameObject;
	}

	public void TakeHit (HitStatus hitStatus, int damage)
	{
		if (hitStatus==HitStatus.Hit) 
		{
            FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.maleGruntMI, transform.position);
			agentStats.TakeDamage(damage);
			localUI.ShowDamage(damage);
			localUI.UpdateHealthBar(agentStats.HitpointPercentage);
			CheckForDeath();
		}
	}

	private void CheckForDeath()
	{
		if (agentStats.CurrentHitpoints < 1) 
		{
			if (dying==false) 
			{
				StartCoroutine("DelayDeath");
				dying = true;
               
            }
		}
	}

	private IEnumerator DelayDeath()
	{
		float delay = 0.5f;
		float counter = 0f;
		while (counter<delay)
		{
			counter += Time.deltaTime;
			yield return null;
		}
		Die();
	}

	private void Die()
	{
        FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.maleDeathMI, transform.position);
        Destroy(self);
	}


}
