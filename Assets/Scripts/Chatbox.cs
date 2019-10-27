using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	//Screen Info
	private InteractionScreen currentScreen;
	private UIoption lastOptionChosen;
	private bool interactable = false;
	[SerializeField] private Text handle;
	[SerializeField] private Image portrait;
	[SerializeField] private Text description;
	[SerializeField] private Button option1;
	[SerializeField] private Button option2;
	[SerializeField] private Button option3;
	[SerializeField] private Color boxColor;

	//Movement and Position
	public enum MoveStatus { Open, Opening, Closed, Closing}
	private MoveStatus status;
	private RectTransform rect;
	private UIPos chatboxPos;
	private float distOpenToClose=Screen.width/4;
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

	public void Open(InteractionScreen openingScreen)
	{ 
		SetChatBoxStatus(MoveStatus.Closed);
		status = MoveStatus.Opening;
		currentScreen = openingScreen;
		interactable = true;
		UpdateScreen();
	}

	public void Close()
	{
		SetChatBoxStatus(MoveStatus.Open);
		status = MoveStatus.Closing;
		interactable = false;
		if (lastOptionChosen.triggerLevelChange && lastOptionChosen.goesToLevel != string.Empty) 
		{
			StartCoroutine("MoveToLevel");
		}
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

	private IEnumerator MoveToLevel()
	{
		float delay = 2f;
		yield return new WaitForSeconds(delay);
		if (lastOptionChosen.triggerLevelChange && lastOptionChosen.goesToLevel != string.Empty)
		{
			LoadingCanvas.ShowLoadingCanvas();
			SceneManager.LoadScene(lastOptionChosen.goesToLevel);
		}
	}

	private void UpdateScreen()
	{
		option1.gameObject.SetActive(false); 
		option2.gameObject.SetActive(false); 
		option3.gameObject.SetActive(false);
		handle.text = currentScreen.handle;
		portrait.sprite = currentScreen.portrait;
		description.text = currentScreen.description;
		boxColor = currentScreen.color;

		if (currentScreen.options.Length >= 1) { option1.gameObject.SetActive(true); }
		if (currentScreen.options.Length >= 2) { option2.gameObject.SetActive(true); }
		if (currentScreen.options.Length >= 3) { option3.gameObject.SetActive(true); }
		if (option1.gameObject.activeSelf) 
		{ 
			option1.GetComponentInChildren<Text>().text = currentScreen.options[0].optionText; 
		}
		if (option2.gameObject.activeSelf) 
		{ 
			option2.GetComponentInChildren<Text>().text = currentScreen.options[1].optionText; 
		}
		if (option3.gameObject.activeSelf) 
		{ 
			option3.GetComponentInChildren<Text>().text = currentScreen.options[2].optionText; 
		}
	}

	public void OptionChoice (int optionNumber)
	{
		if (interactable)
		{
            FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.UISelected);
            if (optionNumber==1) { lastOptionChosen = currentScreen.options[0]; }
			else if (optionNumber == 2) { lastOptionChosen = currentScreen.options[1]; }
			else if (optionNumber == 3) { lastOptionChosen = currentScreen.options[2]; }
			currentScreen.inventory = Inventory.instance;

			if (currentScreen.inventory != null)
			{
				Debug.Log("Inventory called");
				if (currentScreen.itemToReceive != null)
				{

					Inventory.instance.Add(currentScreen.itemToReceive);
				}
			}
			if (lastOptionChosen.lastInteraction)
			{
				Close();
			}
			else 
			{
				currentScreen = lastOptionChosen.optionLeadsTo;
				UpdateScreen();

				
                //Inventory test
                currentScreen.inventory = Inventory.instance; //Inventory test

                
                if (currentScreen.inventory != null)
                {
                    Debug.Log("Inventory called");
                    if (currentScreen.itemToReceive != null)
                    {
                        
                        currentScreen.inventory.Add(currentScreen.itemToReceive);
                    }
                }
                //end Inventory test
				
			}
		}
	}
}
