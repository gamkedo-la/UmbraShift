﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentLocalUI : MonoBehaviour
{
	[SerializeField] private Image nameBox;
	[SerializeField] private Text nameText;
	[SerializeField] private Image infoBox;
	[SerializeField] private Text infoText;
	[SerializeField] private Image feedback;
	[SerializeField] private Text feedbackText;
	private Camera cam;
	private bool showName;
	private bool showInfo;
	

	private void Start()
	{
		cam = Camera.main;
	}

	private void Update()
	{
		if (showName == true)
								{
								nameText.enabled = true;
								Vector3 pos = transform.position + (Vector3.up * 3f);
								nameText.rectTransform.position = cam.WorldToScreenPoint(pos);
								}
		if (showInfo == true)	
								{
								infoText.enabled = true;
								Vector3 pos = transform.position + (Vector3.up * 2f);
								infoText.rectTransform.position = cam.WorldToScreenPoint(pos);
								}
	}

	public void ShowAccuracy()
	{
		showName = ShowText(true, nameText, Color.grey, 12, GetComponent<AgentStats>().CharacterName);
		showInfo = ShowText(true, infoText, Color.white, 12, "70% to hit");
	}

	public void Reset()
	{
		showName = ShowText(false, nameText, Color.grey, 10, "");
		showInfo = ShowText(false, infoText, Color.grey, 10, "");
	}

	private bool ShowText(bool showTheText, Text obj, Color color, int size, string content)
	{
		obj.color = color;
		obj.fontSize = size;
		obj.text = content;
		return showTheText;
	}

}