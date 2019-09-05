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
	private Vector3 targetPos;
	public Vector3 TargetPos { get { return targetPos; } }
	private Color defaultColor;
	public enum RangeCat { None, Optimum, Long, Exceeded }
	public RangeCat rangeToTarget;

	private void Start()
	{
		defaultColor = targetImage.color;
		targetPos = UpdateTargetPos();
	}

	private Vector3 UpdateTargetPos()
	{
		Vector3 pos = transform.position;
		pos.y = pos.y + height;
		return pos;
	}

	private void Update()
	{
		targetPos = UpdateTargetPos();
		if (validTarget && targetImage)
		{
			Vector3 PositionOnScreen = Camera.main.WorldToScreenPoint(targetPos);
			targetImage.rectTransform.position = PositionOnScreen;
		}
		
	}

	public Vector3 ShowTarget()
	{
			targetImage.enabled = true;
			validTarget = true;
			return transform.position;
	}

	public void HideTarget()
	{
		targetImage.color = defaultColor;
		rangeToTarget = RangeCat.None;
		targetImage.enabled = false;
		validTarget = false;
	}

	public void SetColor()
	{
		targetImage.color = defaultColor;
	}

	public void SetColor(Color newColor)
	{
		targetImage.color = newColor;
	}
	

}
