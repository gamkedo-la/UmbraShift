using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentShooting : MonoBehaviour
{
	private bool shootingSystemInUse = false;
	public bool ShootingSystemInUse { get { return shootingSystemInUse; } }
	private List<Targetable> targetList;
	private WeaponDesign weapon;
	private enum ShootingMode { Aiming, Firing, None}
	private ShootingMode shootingMode = ShootingMode.None;

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

	public void ActionStarted()
	{
		shootingSystemInUse = true;
		ResetVariables();
		weapon = GetComponent<AgentStats>().EquippedWeapon;
		PopulateListOfPotentialTargets();
		BeginAiming();
	}

	public void BeginAiming()
	{
		shootingMode = ShootingMode.Aiming;
		//determineRange;
		//draw line to mouse
		//snap mouse to target position if close enough
		//prevent mouse from flying off screen
	}

	public void ResetVariables()		////////////////////
	{
		shootingMode = ShootingMode.None;
		targetList.Clear();
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
				target.ShowTarget();
			}
			else { target.HideTarget(); }
		}
	}



}
