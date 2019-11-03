using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = FilenameDefault, menuName = "UmbraMainSoundConfiguration")]
public class SoundConfiguration : ScriptableObject
{

    #region serialized
    [Header("Agents")]
    [FMODUnity.EventRef]
    public string maleGrunt1;
    [FMODUnity.EventRef]
    public string maleGrunt2;
    [FMODUnity.EventRef]
    public string maleGrunt3;
    [FMODUnity.EventRef]
    public string maleGruntMI;
    [FMODUnity.EventRef]
    public string maleDeath1;
    [FMODUnity.EventRef]
    public string maleDeath2;
    [FMODUnity.EventRef]
    public string maleDeath3;
    [FMODUnity.EventRef]
    public string maleDeathMI;
    [FMODUnity.EventRef]
    public string footSteps1;
    [FMODUnity.EventRef]
    public string footSteps2;
    [FMODUnity.EventRef]
    public string footSteps4;
    [FMODUnity.EventRef]
    public string footSteps5;
    [FMODUnity.EventRef]
    public string enemyAlerted1;
    [FMODUnity.EventRef]
    public string enemyAlerted2;
    [FMODUnity.EventRef]
    public string enemyAlerted3;
    [FMODUnity.EventRef]
    public string playerObjectInteraction1;
    [FMODUnity.EventRef]
    public string playerPickUpObject;

    [FMODUnity.EventRef]
    public string mechDamaged;

    [FMODUnity.EventRef]
    public string mechDestroyed;

    [FMODUnity.EventRef]
    public string mechAttack;


    [Header("UI")]
    [FMODUnity.EventRef]
    public string UISelected;

    [FMODUnity.EventRef]
    public string UISwish;




    [Header("Weapons")]

    [FMODUnity.EventRef]
    public string gunshotPistol1;
    [FMODUnity.EventRef]
    public string gunshotPistol1_2shots;
    [FMODUnity.EventRef]
    public string gunshotPistol1_3shots;
    [FMODUnity.EventRef]
    public string gunshotPistol1_4shots;
    [FMODUnity.EventRef]

    [Header("Objects")]
    public string securityAlarm;
    [FMODUnity.EventRef]
    public string doorSimpleOpening;
    [FMODUnity.EventRef]
    public string doorSimpleClosing;
    [FMODUnity.EventRef]
    public string doorSimpleOpeningWithKeyPad;
    [FMODUnity.EventRef]
    public string cameraMoving;
    [FMODUnity.EventRef]
    public string SlidingDoor;

    [Header("Music")]
    [FMODUnity.EventRef]
    public string inGameMusic;
    #endregion serialized
   
    private static SoundConfiguration _instance;

    private const string FilenameDefault = "UmbraMainSoundConfiguration";


    public static SoundConfiguration instance
    {
        get
        {
            if (null != _instance)
                return _instance;

            _instance = Resources.Load<SoundConfiguration>(FilenameDefault);
            if (null == _instance)
            {
                _instance = CreateInstance<SoundConfiguration>();
                _instance.hideFlags = HideFlags.DontSave;
                Debug.LogError($"Failed to find {typeof(SoundConfiguration)} in Resources -- created temp.", _instance);
            }
            return _instance;
        }
    }

    

    public void playSound(string soundName)
    {
        FMOD.Studio.EventInstance soundEvent = FMODUnity.RuntimeManager.CreateInstance($"event:/{soundName}");
        soundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.gameObject));
        soundEvent.start();
        soundEvent.release();
    }

    public void PlayUISelectSound()
    {
        Debug.Log("Playing UI Select");
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Select");
    }
}
