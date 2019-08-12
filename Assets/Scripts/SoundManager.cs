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


    public string inGameMusic = "event:/InGameMusic";


    private void Awake ( )
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
	
}
