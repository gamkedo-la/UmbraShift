using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitStatus { None, Hit, Miss }
public class CombatReactions : MonoBehaviour
{

	private AgentLocalUI localUI;
	private AgentStats agentStats;
	private bool dying = false;
	private GameObject self;
    public bool givePlayerMissionItemOnDeath=false;
    public Item missionItem;

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
            if (agentStats.isHuman)
            {
                FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.maleGruntMI, transform.position);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.mechDamaged, transform.position);
            }
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
        if (givePlayerMissionItemOnDeath&&missionItem!=null)
        {
            Debug.Log("Giving Player mission Item");
			GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            Inventory playerInventory=playerObject.GetComponent<Inventory>();
            playerInventory.Add(missionItem);
            //FindObjectOfType<InventoryUI>().gameObject.SetActive(true);
        }


        if (agentStats.isHuman)
        {
            FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.maleDeathMI, transform.position);
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.mechDestroyed, transform.position);
        }
		agentStats.Die();
		//Boss boss = GetComponent<Boss>();
		//if (boss) 
		//{ 
		//	FindObjectOfType<DeadBoss>().gameObject.SetActive(true);
		//	FindObjectOfType<DeadBoss>().transform.position = this.transform.position;
		//}

        gameObject.SetActive(false);
		
	}

	


}
