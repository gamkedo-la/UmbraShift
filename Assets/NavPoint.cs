using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavPointType { None, Patrol_Area, Cover_Area}
public class NavPoint : MonoBehaviour
{
	[Header("Reference Only - do not change")]
	[SerializeField] NavPointType type = NavPointType.None;
	[SerializeField] AgentStats agentClaiming = null;
}
