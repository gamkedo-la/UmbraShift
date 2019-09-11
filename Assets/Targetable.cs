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
	[SerializeField] public Image lockOnIndicator;
	[SerializeField] public Text info;

	public bool lockedOn = false;
	public bool LockedOn { get { return lockedOn; } set { lockedOn = value; } }
	private bool validTarget = true;
	public bool ValidTarget { get { return validTarget; } }
	public enum RangeCat { None, Optimum, Long, Exceeded }
	public RangeCat rangeToTarget;
	public enum LOS { Null, Clear, Cover, Blocked }
	public LOS lineOfSight = LOS.Clear;
	public float distance;

	private Vector3 targetPos;
	public Vector3 TargetPos { get { return targetPos; } }
	private Vector2 targetPositionInScreenCoord;
	private Vector3 infoPos;
	public Vector3 InfoPos { get { return infoPos; } }
	private Camera cam;
	private Color defaultColor;

	private void Start()
	{
		targetPos = UpdateTargetPos();
		cam = Camera.main;
		defaultColor = targetImage.color;
	}

	private Vector3 UpdateTargetPos()
	{
		Vector3 pos = transform.position;
		pos.y = pos.y + height;
		return pos;
	}

	private void Update()
	{
		bool posInfoUpdated = false;		
		if (lockedOn && lockOnIndicator != null)
		{
			if (!posInfoUpdated) { UpdatePositionInfo(); posInfoUpdated = true; }
			lockOnIndicator.enabled = true;
			lockOnIndicator.rectTransform.position = targetPositionInScreenCoord;
		}
		else if (!lockedOn && lockOnIndicator && lockOnIndicator.enabled) { lockOnIndicator.enabled = false; }
		if (validTarget && targetImage)
		{
			if (!posInfoUpdated) { UpdatePositionInfo(); posInfoUpdated = true; }
			targetImage.rectTransform.position = targetPositionInScreenCoord;
		}
	}

	private void UpdatePositionInfo()
	{
		targetPositionInScreenCoord = cam.WorldToScreenPoint(targetPos);
	}

	public Vector3 ShowTarget()
	{
		if (!lockedOn) { lockOnIndicator.enabled = false; }
		targetImage.enabled = true;
		validTarget = true;
		return transform.position;
	}

	public void LockOn()
	{
		lockedOn = true;
	}

	public void HideTarget()
	{
		lockedOn = false;
		lockOnIndicator.enabled = false;
		targetImage.color = defaultColor;
		rangeToTarget = RangeCat.None;
		lineOfSight = LOS.Null;
		distance = 0;
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
