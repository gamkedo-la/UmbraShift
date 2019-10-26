using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMusicPlayer : MonoBehaviour
{
    private void PlayerElectricDoorAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.SlidingDoor,gameObject );
    }
}
