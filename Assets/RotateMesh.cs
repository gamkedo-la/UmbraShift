using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMesh : MonoBehaviour
{

    public float xSpeed = 0;
    public float ySpeed = 0;
    public float zSpeed = 50;

    void Update()
    {
        transform.Rotate (
            xSpeed*Time.deltaTime,
            ySpeed*Time.deltaTime,
            zSpeed*Time.deltaTime);
    }
}
