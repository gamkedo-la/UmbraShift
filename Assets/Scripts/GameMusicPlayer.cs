using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicPlayer : MonoBehaviour
{


    private FMOD.Studio.EventInstance inGameMusicEvent;

    
    // Start is called before the first frame update
    void Start()
    {
        inGameMusicEvent = FMODUnity.RuntimeManager.CreateInstance(SoundConfiguration.instance.inGameMusic);
        inGameMusicEvent.start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        inGameMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        UmbraEventManager.AlarmActivated -= ActivateAlarm;
        UmbraEventManager.AlarmDeactivated -= DeactivateAlarm;
    }




    private void OnEnable()
    {
       
        UmbraEventManager.AlarmActivated += ActivateAlarm;
        UmbraEventManager.AlarmDeactivated += DeactivateAlarm;
    }

    public void ActivateAlarm()
    {
        

            inGameMusicEvent.setParameterByName("AlertState", 1f);


        
    }

    public void DeactivateAlarm()
    {
        


            inGameMusicEvent.setParameterByName("AlertState", 0f);

       
    }
}
