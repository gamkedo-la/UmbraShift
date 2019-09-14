using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AgentCombatReactions : MonoBehaviour
{
	[SerializeField] private Canvas combatCanvas;
	[SerializeField] private Text hitResults;
	[SerializeField] private Text scrollingDamage;
	private AgentStats stats;
	private float maxHeight = 2f;
	private enum HitResult { None, Hit, Miss}

    void Start()
    {
		stats = gameObject.GetComponent<AgentStats>();
    }


    public void AttackHit (int damageTaken)
	{
		stats.AdjustHitPoints(-damageTaken);
		if (stats.IsAlive==false) { Die(); }
		ShowHitResult(HitResult.Hit);
	}

	public void AttackMissed()
	{
		ShowHitResult(HitResult.Miss);
	}

	private IEnumerator ShowHitResult(HitResult hitResult)
	{
		float height = 2.25f;
		hitResults.enabled = true;
		//Vector3 defaultPos = hitResults.transform.position;
		//Vector3 currentPos = hitResults.transform.position;
		//currentPos.y += height;
		//hitResults.transform.position = currentPos;
		if (hitResult==HitResult.Hit) 
		{
			hitResults.color = Color.yellow;
			hitResults.text = "Hit";
		}
		else 
		{
			hitResults.color = Color.red;
			hitResults.text = "Miss";
		}
		float counter = 0f;
		float delay = 2f;
		while (counter < delay)
		{
			counter += Time.deltaTime;
			yield return null;
		}
		hitResults.text = "";
		//currentPos = defaultPos;
		//hitResults.transform.position = currentPos;
		hitResults.enabled = false;		
	}

	private IEnumerator ShowDamageTaken(int damage)
	{
		float height = 0f;
		scrollingDamage.enabled = true;
		scrollingDamage.color = Color.red;
		scrollingDamage.text = "-" + damage.ToString();
		//Vector3 defaultPos = scrollingDamage.transform.position;
		//Vector3 currentPos = scrollingDamage.transform.position;
		float counter = 0f;
		float delay = 2f;
		while (counter < delay)
		{
			counter += Time.deltaTime;
		//	height = height + (maxHeight / delay) * Time.deltaTime;
		//	currentPos.y = currentPos.y + height;
		//	scrollingDamage.transform.position = currentPos;
			yield return null;
		}
		//currentPos = defaultPos;
		//scrollingDamage.transform.position = currentPos;
		//height = 0f;
		scrollingDamage.text = "";
		scrollingDamage.enabled = false;
	}

	private void Die()
	{
		Destroy(this.gameObject);
	}

}
