using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;

    public string maleGrunt1 = "event:/Male_Grunt_1";
    public string maleGrunt2 = "event:/Male_Grunt_2";
    public string maleGrunt3 = "event:/Male_Grunt_3";
    public string maleGruntMI = "event:/HumanMaleGruntMI";
    public string footSteps1 = "event:/HumanFootSteps_1";
    public string footSteps2 = "event:/HumanFootSteps_2";
    public string gunshotPistol1 = "event:/Gun_Shot_Pistol_1";
    public string gunshotPistol1_2shots = "event:/Gun_Shot_Pistol_1-burst2";
    public string gunshotPistol1_3shots = "event:/Gun_Shot_Pistol_1-burst3";
    public string gunshotPistol1_4shots = "event:/Gun_Shot_Pistol_1-burst4";
    public string enemyAlerted1 = "event:/Alerted_Guard_1";
    public string enemyAlerted2 = "event:/Alerted_Guard_2";
    public string enemyAlerted3 = "event:/Alerted_Guard_3";
    public string securityAlarm = "event:/Security Alarm";


    public string inGameMusic = "event:/InGameMusic";


    private void Start ( )
	{
		if ( instance != null )
		{
			Debug.Log( "singleton already existed but attempted re-assignment" );
		}
		else
		{
			instance = this;
		}
	}

	public void playSound(string soundName)
	{
		FMOD.Studio.EventInstance soundEvent = FMODUnity.RuntimeManager.CreateInstance( $"event:/{soundName}");
		soundEvent.set3DAttributes( FMODUnity.RuntimeUtils.To3DAttributes( Camera.main.gameObject ) );
		soundEvent.start( );
	}

    public void PlayUISelectSound()
    {
        Debug.Log("Playing UI Select");
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Select");
    }
	
}
