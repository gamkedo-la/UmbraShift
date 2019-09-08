using System.Collections;
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
	[SerializeField] private Text coverText;
	private Camera cam;
	private bool showName;
	private bool showInfo;
	private bool showCover;

	private void Start()
	{
		cam = Camera.main;
		coverText.text = "Cover Penalty";
		coverText.color = Color.red;
		coverText.enabled = false;
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
		if (showCover == true)	{
								coverText.enabled = true;
								Vector3 pos = transform.position + (Vector3.up * 4);
								coverText.rectTransform.position = cam.WorldToScreenPoint(pos);
								}
	}

	public void ShowAccuracy(int accuracy)
	{
		showName = ShowText(true, nameText, Color.grey, 12, GetComponent<AgentStats>().CharacterName);
		showInfo = ShowText(true, infoText, Color.white, 12, accuracy.ToString() + "% to hit");
	}

	public void ShowCoverNotice(int penalty)
	{
		showCover = ShowText(true, coverText, Color.red, 12, "-" + penalty.ToString() + " cover penalty");
	}

	public void Reset()
	{
		showName = ShowText(false, nameText, Color.grey, 10, "");
		showInfo = ShowText(false, infoText, Color.grey, 10, "");
		showCover = ShowText(false, coverText, Color.red, 10, "");
	}

	private bool ShowText(bool showTheText, Text obj, Color color, int size, string content)
	{
		obj.color = color;
		obj.fontSize = size;
		obj.text = content;
		return showTheText;
	}

}
