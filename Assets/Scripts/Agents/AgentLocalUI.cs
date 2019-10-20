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
	[SerializeField] private Image lockedOnIndicator;
	[SerializeField] private Image healthBar;
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
		UpdateHealthBar(1f);
		StartCoroutine("DelayedStart");
	}

	private IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(1f);
		AgentStats agentStats = GetComponent<AgentStats>();
		if (agentStats) { UpdateHealthBar(agentStats.HitpointPercentage);}
		else { Debug.LogWarning(gameObject.name + " is missing an AgentStats component."); }
	}

	private void Update()
	{
		if (healthBar)
								{
									Vector3 healthBarPos = transform.position + (Vector3.up * 1.5f);
									healthBar.rectTransform.parent.position = cam.WorldToScreenPoint(healthBarPos);
								}
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
		if (obj)
		{
			obj.color = color;
			obj.fontSize = size;
			obj.text = content;
		}
		return showTheText;
	}

	public void UpdateHealthBar(float percentage)
	{
		percentage = Mathf.Clamp(percentage, 0f, 1f);
		if (healthBar)
		{
			healthBar.fillAmount = percentage;
		}
		else { Debug.LogWarning(gameObject.name + " is missing a HealthBar property on its LocalUI component."); }
	}

	public void ShowDamage (int damageNum)
	{
		StartCoroutine("ShowScrollingText", damageNum);
	}

	private IEnumerator ShowScrollingText(int damageNum)
	{
		feedbackText.color = Color.red;
		feedbackText.fontSize = 16;
		feedbackText.text = "-" + damageNum.ToString();
		feedbackText.enabled = true;
		float height = 0f;
		float counter = 0f;
		float speed = 3f;
		float delay = 0.75f;
		while (counter<delay)
		{
			counter += Time.deltaTime;
			height = height + (Time.deltaTime * speed);
			Vector3 pos = transform.position + (Vector3.up * (2f + height));
			feedbackText.rectTransform.position = cam.WorldToScreenPoint(pos);
			yield return null;
		}
		feedbackText.enabled = false;
	}

}
