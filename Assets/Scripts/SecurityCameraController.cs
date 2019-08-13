using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{

    public void PlayCameraMovementAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Camera_Moving");
    }
}
