using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbraEventManager : MonoBehaviour
{


    public delegate void Alarm();
    public static event Alarm AlarmActivated;
    public static event Alarm AlarmDeactivated;
    public static UmbraEventManager instance;
    public bool alertState =false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ActivateAlarm()
    {
        if (AlarmActivated != null)
        {
            AlarmActivated();
            alertState = true;
           
        }
    }

    public void DeactivateAlarm()
    {
        if (AlarmActivated != null)
        {
            AlarmDeactivated();
            alertState = false;

        }
    }





}
