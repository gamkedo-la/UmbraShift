using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{

	// float movementPoints;
	// float actionPoints;
	[SerializeField] private Sprite portraitImage;
	[SerializeField] private string characterName;
	public Sprite PortraitImage { get { return portraitImage; } }
	public string CharacterName { get { return characterName; } }

    
	public Sprite GetPortraitImage()
	{
		return portraitImage;
	}

	public string GetCharacterName()
	{
		return characterName;
	}

	void Start()
    {
        
    }

    void Update()
    {
        
    }
}
