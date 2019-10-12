using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public void PlayWalkingSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.footSteps4,gameObject);
    }

    public void PlaySound(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(path, gameObject);
    }
}
