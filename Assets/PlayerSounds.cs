using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public void PlayWalkingSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(SoundConfiguration.instance.footSteps4);
    }
}
