using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayMenuMusic : MonoBehaviour
{
    private FMOD.Studio.EventInstance menuMusicEvent;
    private int randomLoop;

    void Awake() 
    {
        randomLoop = Random.Range(0, 3);
        menuMusicEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Menu_remixes");
    }

    // Start is called before the first frame update
    void Start()
    {
        menuMusicEvent.setParameterByName("Random Menu Mix", randomLoop);
        menuMusicEvent.start();
    }

    private void OnDestroy()
    {
        menuMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
