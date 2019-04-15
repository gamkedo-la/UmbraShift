using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    // because this relies upon camera POS want to happen after any camera calls
    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
