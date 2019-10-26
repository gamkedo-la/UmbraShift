using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	GameObject deadBoss;
	
    void Start()
    {
		deadBoss = FindObjectOfType<DeadBoss>().gameObject;
		deadBoss.SetActive(false);
    }

	public void OnDisable()
	{
		if (deadBoss)
		{
			deadBoss.SetActive(true);
			deadBoss.transform.position = this.transform.position;
		}
	}

	public void OnDestroy()
	{
		if (deadBoss)
		{
			deadBoss.SetActive(true);
			deadBoss.transform.position = this.transform.position;
		}
	}

}
