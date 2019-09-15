using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentTurnManager : MonoBehaviour
{
	[SerializeField] private AgentStats mainPlayerCharacter = null;
	[SerializeField] private AgentStats altPlayerCharacter = null;
	private AgentStats activeCharacter = null;
	public AgentStats ActiveCharacter { get { return activeCharacter; } }

	public AgentStats GetActiveCharacter()
	{
		return activeCharacter;
	}

    void Start()
    {
		activeCharacter = mainPlayerCharacter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
