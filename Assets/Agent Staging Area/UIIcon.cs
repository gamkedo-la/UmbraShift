using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Action { None, Move, Shoot, Burst, AimedShot, Umbra, Grenade, Medical, Interact, Hack, Cancel, Undo, Continue}
public class UIIcon : MonoBehaviour
{
	[SerializeField] private int actionIconNum = 0;
	[SerializeField] private string iconName;
	[SerializeField] private Action action = Action.None;
	[SerializeField] private Image portrait;
	[SerializeField] private Image frame;
	[SerializeField] private Text textBox;
	[SerializeField] public float driftTime = 0.3f;
	[HideInInspector] public Vector3 homePosition;
	[HideInInspector] public Vector3 hidePosition;
	[SerializeField] public bool hidden;
	public Action ActionWhenPressed { get { return action; } }
	public int ActionIconNum { get { return actionIconNum; } }
	public const float FLASH_DURATION = 0.15f;
	public const float FLASH_CYCLES = 2;

	public float GetFlashDelay()
	{
		return FLASH_DURATION * FLASH_CYCLES;
	}

	public void Flash (Color color)
	{
		StartCoroutine (FlashFrameColor(color));
	}

	private IEnumerator FlashFrameColor(Color colorToFlash)
	{
		Color defaultColor = frame.color;
		float counter = 0f;
		float duration = FLASH_DURATION;
		for (int i = 0; i < FLASH_CYCLES; i++)
		{
			if (frame.color == colorToFlash) { frame.color = defaultColor; }
			else if (frame.color == defaultColor) { frame.color = colorToFlash; }
			counter = 0f;
			while (counter < duration)
			{
				counter = counter + Time.deltaTime;
				yield return null;
			}
		}
		frame.color = defaultColor;
	}

	public void SetHidePosition(Vector3 position)
	{
		hidePosition = position;
	}

	public void SetHomePosition(Vector3 position)
	{
		homePosition = position;
	}

}
