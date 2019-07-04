﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public PlayerController currentPlayerController;
    public Material highlightedMat;
    public Transform playerMoveTarget;
    public Interactable lastUnderMouse;
	public GameObject playerSelectIndicator;

    public UIController uiController;
	private float playerHeight = 1.05f;
    
	void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Tried to assign instance when already exists.");
        }
        else
        {
            instance = this;
        }
    }

    public void SetCurrentPlayerController(PlayerController playerController)
    {
        currentPlayerController = playerController;
        playerController.SetAsActivePlayerController();
        Debug.Log($"Set new manager from {currentPlayerController.name}");
		SetSelectionIndicatorOnPlayer(playerController);
		BaseCharacterClass BCC = currentPlayerController.gameObject.GetComponent<BaseCharacterClass>();
        if (BCC != null)
        {
            Debug.Log(BCC.avatar);
            uiController.SetCurrentCharacterName(BCC.characterName);
            uiController.SetCurrentCharacterAvatar(BCC.avatar);
        }
    }

	private void SetSelectionIndicatorOnPlayer(PlayerController playerController)
	{
		playerSelectIndicator.SetActive(true);
		playerSelectIndicator.transform.position = playerController.transform.position;
		playerSelectIndicator.transform.position = new Vector3(
												playerController.transform.position.x,
												playerController.transform.position.y - playerHeight,
												playerController.transform.position.z);
		playerSelectIndicator.transform.SetParent(playerController.transform);
	}

	public void SingleShotFromActivePlayer()
    {
        Debug.Log("Calling single shot on active player");
        currentPlayerController.SingleShot();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPlayerController = null;
		playerSelectIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("I pressed C");
            TurnManager.instance.CombatMode();
        }
        if (currentPlayerController == null)
        {
            //Debug.Log("Action manager is null");
            return;
        }

        RaycastHit rhinfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhinfo, 100, ~LayerMask.GetMask("HideRoomBlackOutBoxFromEditor")))
        {
            if (Input.GetMouseButtonDown(0))
            {
//                Debug.Log($"Layer is {rhinfo.transform.gameObject.layer} and mask {LayerMask.NameToLayer("VisibleNPC")}");
                if (rhinfo.transform.gameObject.layer == LayerMask.NameToLayer("VisibleNPC"))
                {
                    Debug.Log($"Clicked on {rhinfo.transform.name}");
                    currentPlayerController.SetTarget(rhinfo.transform);
                }
                else
                {
                    float moveDist = Vector3.Distance(rhinfo.point,currentPlayerController.transform.position);
                    int cost = Mathf.FloorToInt(moveDist / 10);
                    if (cost == 0)
                    {
                        cost = 1;
                    }
                    if (TurnManager.instance.isCombatModeActive == false || currentPlayerController.AttemptToSpend(cost, true))
                        // this is order dependent and we need to check for is combat mode active first 
                    {
                        playerMoveTarget.transform.position = rhinfo.point;
                    }
                    else
                    {
                        Debug.Log("Not enough action points to move!");
                    }
                    Debug.Log(moveDist);
                }
            }
            else
            {
                //Debug.Log($"{rhinfo.transform.name}");
                Interactable currentUnderMouse = rhinfo.transform.gameObject.GetComponent<Interactable>();
                if (currentUnderMouse != null)
                {
//                    Debug.Log($"current under mouse is {currentUnderMouse}");
                    if (currentUnderMouse != lastUnderMouse)
                    {
                        Debug.Log("Current and last were not the same.");
                        if (lastUnderMouse != null)
                        {
                            lastUnderMouse.ObjectDeselected();
                        }
                        lastUnderMouse = currentUnderMouse;
                    }
                    else
                    {
//                        Debug.Log("Highlight yo self!");
                        currentUnderMouse.ObjectSelected();
                    }
                }
                else if (lastUnderMouse != null)
                {
                    lastUnderMouse.ObjectDeselected();
                }
            }
        }
    }
}
