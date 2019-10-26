﻿using System.Collections;
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
		deadBoss.SetActive(true);
		deadBoss.transform.position = this.transform.position;
	}

	public void OnDestroy()
	{
		deadBoss.SetActive(true);
		deadBoss.transform.position = this.transform.position;
	}

}
