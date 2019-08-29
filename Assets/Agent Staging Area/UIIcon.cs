using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{
	[SerializeField] private int actionIconNum = 0;
	[SerializeField] private string name;
	[SerializeField] private Image portrait;
	[SerializeField] private Image frame;
	[SerializeField] private Text textBox;
	[SerializeField] public float driftTime = 0.5f;
	[SerializeField] private ActionInProcess actionType;
	[HideInInspector] public Vector3 homePosition;
	[HideInInspector] public Vector3 hidePosition;
	[SerializeField] public bool hidden;
	public int ActionIconNum { get { return actionIconNum; } }
	

	public void SetHidePosition(Vector3 position)
	{
		hidePosition = position;
	}

	public void SetHomePosition(Vector3 position)
	{
		homePosition = position;
	}

}
