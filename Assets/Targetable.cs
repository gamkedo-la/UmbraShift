using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targetable : MonoBehaviour
{
	[SerializeField] private Image targetImage;
	[SerializeField] public bool Shootable;
	[SerializeField] public bool Interactable;
	[SerializeField] public bool Hackable;
	[SerializeField] public float height;
	private bool validTarget = true;
	public bool ValidTarget { get { return validTarget; } }
	
	private void Update()
	{
		if (validTarget && targetImage)
		{
			Vector3 pos = transform.position;
			pos.y = pos.y + height;
			Vector3 PositionOnScreen = Camera.main.WorldToScreenPoint(pos);
			targetImage.rectTransform.position = PositionOnScreen;
		}
		else { Debug.Log(gameObject.name + " Targetable script is missing information."); }
	}

	public Vector3 ShowTarget()
	{
			targetImage.enabled = true;
			validTarget = true;
			return transform.position;
	}

	public void HideTarget()
	{
		targetImage.enabled = false;
		validTarget = false;
	}

	

}
