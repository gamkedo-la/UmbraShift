using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public ActionManager actionManager;
    public Material highlightedMat;
    public Transform playerMoveTarget;
    public Interactable lastUnderMouse;

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

    public void SetCurrentActionManager(ActionManager manager)
    {
        actionManager = manager;
        Debug.Log($"Set new manager from {manager.name}");
    }

    // Start is called before the first frame update
    void Start()
    {
        actionManager = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (actionManager == null)
        {
            //Debug.Log("Action manager is null");
            return;
        }
        RaycastHit rhinfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhinfo, 100))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Layer is {rhinfo.transform.gameObject.layer} and mask {LayerMask.NameToLayer("VisibleNPC")}");
                if (rhinfo.transform.gameObject.layer == LayerMask.NameToLayer("VisibleNPC"))
                {
                    Debug.Log($"Clicked on {rhinfo.transform.name}");
                    if (actionManager.AttemptToSpend(3, true))
                    {
                        Debug.Log("Bang bang");
                    }
                    else
                    {
                        Debug.Log("Not enough AP");
                    }
                }
                else
                {
                    playerMoveTarget.transform.position = rhinfo.point;
                }
            }
            else
            {
                //Debug.Log($"{rhinfo.transform.name}");
                Interactable currentUnderMouse = rhinfo.transform.gameObject.GetComponent<Interactable>();
                if (currentUnderMouse != null)
                {
                    Debug.Log($"current under mouse is {currentUnderMouse}");
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
                        Debug.Log("Highlight yo self!");
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
