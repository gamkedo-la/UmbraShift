using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Quality { None, Terrible=-20, Average=-10, Effective=10, Superior=20 }
public enum Damage { None=0, Terrible=5, Average=8, Effective=10, Superior=12}
//public enum Hinderance { None, Negligable, Average, Hindering, Severe}
public enum MaxRange { Adjacent = 3, Short = 12, Medium = 21, Long = 30 }
public enum AmmoType { Type_IV_Biogel_Rounds, Biohazard_Gas_X3 }
//public enum FiringMode { Single_Shot_Only, Burst_Fire}
//public enum TargetingMode { Direct_Fire_Only, Aimed_Shot, Smartlink_Equipped}
public enum CorpName { Omegatech_Munitions, AHO_AeroSpace, Tsu_Mura_Technology_Multinational, Kura_Industrial, Evotech_Global,
						NewLink_Research_and_Medical, MilDev_Unlimited, Ying_Integrated_Cybernetics, TNW_Entertainment,
						ExaCorp_Technologies, Sabine_Corporation, Markell_Technology_Consolidated, Qiu_Armament, SDT_Corporation}
[CreateAssetMenu(fileName = "CRs-4 Utility Pistol", menuName = "Weapon Design")]
public class WeaponDesign : ScriptableObject
{
	public string officialName = "CRs-4 Utility Pistol";
	public CorpName manufacturer = CorpName.Kura_Industrial;
	[TextArea] public string description = "Small and cheap to manufacture, this no-frills pistol is sold in bulk and used by " +
 										"security guards and rent-a-cops throughout the world. Cheap and easy to get, this " +
										"weapon is also favored by street gangs and consumer hobbyists.";
	public Sprite sprite;
	[Space]
	public ItemType weaponType = ItemType.Pistol;
	public AmmoType ammo = AmmoType.Type_IV_Biogel_Rounds;
	public GameObject projectilePrefab;
	[Space]
	public int cost = 0;
	[Space]
	public Damage damage = Damage.Terrible;
	public MaxRange range = MaxRange.Short;
	public Quality accuracy = Quality.Average;
	//public TargetingMode targeting = TargetingMode.Direct_Fire_Only;

	//public FiringMode firingMode = FiringMode.Single_Shot_Only;
	//public Hinderance recoil = Hinderance.Negligable;
}
