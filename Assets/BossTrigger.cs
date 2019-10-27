using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
	[SerializeField] private InteractionScreen interaction;
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag=="Player")
		{
			PlayerCharacterData playerCharacterData = FindObjectOfType<PlayerCharacterData>();
			if (playerCharacterData) { playerCharacterData.AdvanceStory(); }
			Chatbox chatBox = FindObjectOfType<Chatbox>();
			if (chatBox) { chatBox.Open(interaction); }
			Destroy(this.gameObject);
		}
	}
}
