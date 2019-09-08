using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityAlarmController : MonoBehaviour
{

    private FMOD.Studio.EventInstance securityAlarmEvent;

    private void OnDisable()
    {
       
        UmbraEventManager.AlarmActivated -= ActivateAlarm;
        UmbraEventManager.AlarmDeactivated -= DeactivateAlarm;
    }




    private void OnEnable()
    {

        UmbraEventManager.AlarmActivated += ActivateAlarm;
        UmbraEventManager.AlarmDeactivated += DeactivateAlarm;
    }


   

    void ActivateAlarm()
    {


        if (!securityAlarmEvent.IsPlaying())
        {
            securityAlarmEvent = FMODUnity.RuntimeManager.CreateInstance(SoundConfiguration.instance.securityAlarm);
            securityAlarmEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            securityAlarmEvent.start();
        }
            
    }

    void DeactivateAlarm()
    {


        securityAlarmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
