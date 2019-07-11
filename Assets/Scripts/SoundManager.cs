using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;

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
		FMOD.Studio.EventInstance soundEvent = FMODUnity.RuntimeManager.CreateInstance( soundName );
		soundEvent.set3DAttributes( FMODUnity.RuntimeUtils.To3DAttributes( Camera.main.gameObject ) );
		soundEvent.start( );
	}
	
}
