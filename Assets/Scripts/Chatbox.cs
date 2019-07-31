using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct UIPos
{
	public Vector2 closedPosMin;
	public Vector2 closedPosMax;
	public Vector2 openPosMin;
	public Vector2 openPosMax;
	public Vector2 openingVector;
	public Vector2 closingVector;
	public float distance;
	public float openingSpeed;
	public float closingSpeed;

	public UIPos (Vector2 openMin, Vector2 openMax, Vector2 closing)
	{
		openPosMin = openMin;
		openPosMax = openMax;
		closingVector = closing;
		openingVector = -closing;
		closedPosMin = openMin + closing;
		closedPosMax = openMax + closing;
		distance = closing.magnitude;
		openingSpeed = 2.5f;
		closingSpeed = 2.5f;
	}


}


public class Chatbox : MonoBehaviour
{
	public enum MoveStatus { Open, Opening, Closed, Closing}
	private MoveStatus status;
	private RectTransform rect;
	private UIPos chatboxPos;
	private float distOpenToClose=Screen.width/5;
	private const float DIST_THRESHOLD = 0.1f;

    void Start()
    {
		rect = GetComponent<RectTransform>();
		chatboxPos = new UIPos(rect.offsetMin, rect.offsetMax, Vector2.right * distOpenToClose);
		SetChatBoxStatus(MoveStatus.Closed);
	}

	public void SetChatBoxStatus(MoveStatus moveStatus)
	{
		if (moveStatus == MoveStatus.Open)
		{
			rect.offsetMin = chatboxPos.openPosMin;
			rect.offsetMax = chatboxPos.openPosMax;
			status = MoveStatus.Open;
			return;
		}
		else 
		{
			rect.offsetMin = chatboxPos.closedPosMin;
			rect.offsetMax = chatboxPos.closedPosMax;
			return;
		}
	}
    
    void Update()
    {
        if (status == MoveStatus.Opening) 
		{
			Move(chatboxPos.openingVector, chatboxPos.openingSpeed, chatboxPos.closedPosMin);
		}
		if (status == MoveStatus.Closing) 
		{ 
			Move (chatboxPos.closingVector, chatboxPos.closingSpeed, chatboxPos.openPosMin);
		}
	}

	public void Open()
	{ 
		SetChatBoxStatus(MoveStatus.Closed);
		status = MoveStatus.Opening;
	}

	public void Close()
	{
		SetChatBoxStatus(MoveStatus.Open);
		status = MoveStatus.Closing;
	}

	private void Move(Vector2 direction, float speed, Vector2 origin)
	{
		rect.offsetMin = rect.offsetMin + (direction * speed * Time.deltaTime);
		rect.offsetMax = rect.offsetMax + (direction * speed * Time.deltaTime);
		CheckForDestination(origin);
	}

	private void CheckForDestination(Vector2 origin)
	{
		float dist = Vector2.Distance(rect.offsetMin, origin);
		if (dist >= chatboxPos.distance) 
		{ 
			if (status == MoveStatus.Opening) 	{ SetChatBoxStatus(MoveStatus.Open);   }
			else if (status == MoveStatus.Closing) 	{ SetChatBoxStatus(MoveStatus.Closed); }
		}
	}


}
