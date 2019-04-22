using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTarget : MonoBehaviour
{
    private ActionManager actionManager;
    // Start is called before the first frame update
    void Start()
    {
        actionManager = PlayerController.instance.GetComponent<ActionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit rhinfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhinfo, 100))
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
                    transform.position = rhinfo.point;
                }

            }
        }
    }
}
