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
        StartCoroutine(StartMusic());

        //FMODUnity.RuntimeManager.GetBus("bus:/Music").setVolume(PlayerPrefs.GetFloat("MusicVolume", 75.0F));


    }

    // Start is called before the first frame update
    void Start()
    {




       
    }

    private void OnDestroy()
    {
        menuMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(1);

        if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {
            randomLoop = Random.Range(0, 3);
            menuMusicEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Menu_remixes");
            menuMusicEvent.setParameterByName("Random Menu Mix", randomLoop);
            menuMusicEvent.start();
            menuMusicEvent.release();

        }
        else
        {
            StartCoroutine(StartMusic());
        }


    }
}
