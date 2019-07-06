using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;

	// FMOD events
	[FMODUnity.EventRef]
	public string MouseDownEvent;
	FMOD.Studio.EventInstance MouseDown;

	[FMODUnity.EventRef]
	public string MouseUpEvent;
	FMOD.Studio.EventInstance MouseUp;

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
		MouseDown = FMODUnity.RuntimeManager.CreateInstance( MouseDownEvent );
		MouseDown.set3DAttributes( FMODUnity.RuntimeUtils.To3DAttributes( Camera.main.gameObject ) );
		MouseDown.start( );
	}
	
}
