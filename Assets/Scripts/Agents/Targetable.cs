using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targetable : MonoBehaviour
{
	[SerializeField] public bool Shootable;
	[SerializeField] public bool Interactable;
	[SerializeField] public bool Hackable;
	[SerializeField] private Image targetImage;
	[SerializeField] public Image lockOnIndicator;
	[SerializeField] public Text info;

	private bool lockedOn = false;
	[HideInInspector] public bool LockedOn { get { return lockedOn; } }
	private bool validTarget = true;
	[HideInInspector] public float distance;
	[HideInInspector] public bool ValidTarget { get { return validTarget; } }
	[HideInInspector] public enum RangeCat { None, Optimum, Long, Exceeded }
	[HideInInspector] public RangeCat rangeToTarget;
	[HideInInspector] public enum LOS { Null, Clear, Cover, Blocked }
	[HideInInspector] public LOS lineOfSight = LOS.Clear;

	[HideInInspector]public float height = 1f;
	private Vector3 targetPos;
	public Vector3 TargetPos { get { return targetPos; } }
	private Vector2 targetPositionInScreenCoord;
	private Vector3 infoPos;
	public Vector3 InfoPos { get { return infoPos; } }
	private Camera cam;
	private Color defaultColor;
	private Color selectionHoldColor;
	private enum SelectionStatus { Clear, Selected, Held, Activated}
	private SelectionStatus selected = SelectionStatus.Clear;

	private void Awake()
	{
		targetPos = UpdateTargetPos();
		cam = Camera.main;
		defaultColor = targetImage.color;
		selectionHoldColor = Color.green;
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
		targetPositionInScreenCoord = cam.WorldToScreenPoint(targetPos);
		if (validTarget && targetImage) 
		{ 
			targetImage.rectTransform.position = targetPositionInScreenCoord; 
		}
		if (selected == SelectionStatus.Selected && lockOnIndicator) 
		{
			lockOnIndicator.enabled = true;
			lockOnIndicator.rectTransform.position = targetPositionInScreenCoord;
		}
		if (selected==SelectionStatus.Clear && lockOnIndicator && lockOnIndicator.enabled) 
		{
			lockOnIndicator.color = defaultColor;
			lockOnIndicator.enabled = false; 
		}
		//if (lockedOn && lockOnIndicator && selected == SelectionStatus.Selected && 
		//	Input.GetMouseButtonDown(0) && lockOnIndicator.enabled &&
		//	Vector3.Distance(Input.mousePosition, cam.WorldToScreenPoint(targetPos)) < 1f) 
		//{
		//	selected = SelectionStatus.Held;
		//	lockOnIndicator.color = Color.green;
		//}
	}


	public Vector3 ShowTarget()
	{
		if (!lockedOn && lockOnIndicator) { lockOnIndicator.enabled = false; }
		targetImage.enabled = true;
		validTarget = true;
		return transform.position;
	}

	public void Select()
	{
		lockedOn = true;
		selected = SelectionStatus.Selected;
	}

	public void HoldSelection()
	{
		selected = SelectionStatus.Held;
		lockOnIndicator.color = selectionHoldColor;
	}

	public void SelectionClear()
	{
		lockedOn = false;
		selected = SelectionStatus.Clear;
		lockOnIndicator.color = defaultColor;
		lockOnIndicator.enabled = false;
	}

	public void HideTarget()
	{
		lockedOn = false;
		if (lockOnIndicator != null)
        {
			lockOnIndicator.color = defaultColor;
			lockOnIndicator.enabled = false;
		}
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
