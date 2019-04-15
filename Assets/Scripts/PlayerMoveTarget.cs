using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTarget : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit rhinfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhinfo, 100))
            {
                transform.position = rhinfo.point;
            }
        }
    }
}
