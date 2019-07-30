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
	private Vector2 openingVector;
	private Vector2 closingVector;
	private float distance;
	private const float OPENING_SPEED = 1f;
	private const float CLOSING_SPEED = 1f;

	public UIPos (Vector2 openMin, Vector2 openMax, Vector2 opening)
	{
		openPosMin = openMin;
		openPosMax = openMax;
		closedPosMin = openMin + opening;
		closedPosMax = openMax + opening;
		openingVector = opening;
		closingVector = -opening;
		distance = opening.magnitude;
	}


}


public class Chatbox : MonoBehaviour
{
	public enum MoveStatus { Open, Opening, Closed, Closing}
	private MoveStatus status;
	private RectTransform rect;
	private UIPos chatBoxPos;
	private const float CLOSED_DISTANCE=20f;


    void Start()
    {
		rect = GetComponent<RectTransform>();
		chatBoxPos = new UIPos(rect.offsetMin, rect.offsetMax, Vector2.right * CLOSED_DISTANCE);
		SetChatBoxStatus(MoveStatus.Closed);
	}

	public void SetChatBoxStatus(MoveStatus moveStatus)
	{
		if (moveStatus == MoveStatus.Open)
		{
			rect.offsetMin = chatBoxPos.openPosMin;
			rect.offsetMax = chatBoxPos.openPosMax;
			status = MoveStatus.Open;
			return;
		}
		else 
		{
			rect.offsetMin = chatBoxPos.closedPosMin;
			rect.offsetMax = chatBoxPos.closedPosMax;
			return;
		}
	}
    
    void Update()
    {
        //if (status == MoveStatus.Opening){ MoveChatbox (some arguments);}
		//if (status == MoveStatus.Closing) { MoveChatbox (some arguments);}
	}

	public void Open()
	{ 
		SetChatBoxStatus(MoveStatus.Closed);
		//MoveChatBox (chatBoxPos.openingVector, chatBoxPos.OPENING_SPEED, chatboxPos.OpenPosMin);
	}

	public void Close()
	{
		SetChatBoxStatus(MoveStatus.Open);
		//MoveChatbox (chatboxPos.closingVector, chatboxPos.CLOSING_SPEED, chatboxPos.ClosedPosMin);
	}

	private void MoveChatbox(Vector2 direction, float speed, Vector2 destination)
	{
		//TODO: consider changing MoveChatBox to a MoveUIPos that can be used by any other UI scripts with the struct
		//TODO: consider using a LERP value for moveChatBox instead of speed
		CheckForDestination(destination);
	}

	private void CheckForDestination(Vector2 destination)
	{
		//check to see if the chatbox is close to where it's supposed to end up
		//if it is, change its status to a new MoveStatus and use SetChatboxPos to set its final place
	}


}
