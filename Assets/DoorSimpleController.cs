using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSimpleController : MonoBehaviour
{
   public void PlayOpeningSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.instance.doorSimpleOpening);
    }

    public void PlayClosingSound()
    {

    }
}
