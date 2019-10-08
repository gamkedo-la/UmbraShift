using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerBoomMover : MonoBehaviour
{
    public Transform mainCameraTransform;
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;
    private void Start()
    {
        if (mainCameraTransform == null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (mainCameraTransform != null)
        {
            transform.position = new Vector3(mainCameraTransform.position.x + offsetX, mainCameraTransform.position.y+offsetY, mainCameraTransform.position.z + offsetZ);
        }
    }
}
