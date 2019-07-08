using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGrid : MonoBehaviour
{
    public float rows = 10.0f;

    public float columns = 10.0f;

// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color lineColor = new Color(1.0f, 1.0f, 0.0f);
        for (int i = 0; i < rows; i++)
        {
            Debug.DrawLine(transform.position + Vector3.right * i, transform.position + Vector3.forward * columns + Vector3.right * i, lineColor);
        }
        
        for (int i = 0; i < columns; i++)
        {
            Debug.DrawLine(transform.position + Vector3.forward * i, Vector3.right * rows + transform.position + Vector3.forward * i, lineColor);
        }
        
    }
}
