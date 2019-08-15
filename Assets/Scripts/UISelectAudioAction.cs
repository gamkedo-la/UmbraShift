using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectAudioAction : MonoBehaviour
{
    public void PlayUISelectSound()
    {
        Debug.Log("Playing UI Select",this);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Select");
    }
}
