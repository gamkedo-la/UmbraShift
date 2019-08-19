using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenActions : MonoBehaviour
{

    public Slider masterVolumeSlider;
    public GameObject pauseMenuPanel;


    FMOD.Studio.Bus musicBus;
    FMOD.Studio.Bus sfxBus;


    private void Awake()
    {
        musicBus= FMODUnity.RuntimeManager.GetBus("bus:/Music");
        sfxBus= FMODUnity.RuntimeManager.GetBus("bus:/SoundFx");
    }








    public void UpdateMusicVolume(float value)
    {

       
        musicBus.setVolume(value);

    }


    public void UpdateSoundFxVolume(float value)
    {


        sfxBus.setVolume(value);
       

    }



    public void UpdateMasterVolume(float value)
    {

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Master Volume",value);
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)){

            if (pauseMenuPanel.activeInHierarchy)
            {
                pauseMenuPanel.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                pauseMenuPanel.SetActive(true);
                Time.timeScale = 0f;
            }

        }
    }






}
