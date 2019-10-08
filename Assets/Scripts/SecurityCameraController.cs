using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{

    public void PlayCameraMovementAudio()
    {
        Debug.Log("Playing Camera Audio");
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.cameraMoving,gameObject);
    }
}
