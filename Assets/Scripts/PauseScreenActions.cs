using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreenActions : MonoBehaviour
{

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public GameObject pauseMenuPanel;


    FMOD.Studio.Bus musicBus;
    FMOD.Studio.Bus sfxBus;
    FMOD.Studio.Bus masterBus;


    private void Awake()
    {
        musicBus= FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        sfxBus= FMODUnity.RuntimeManager.GetBus("bus:/Master/SoundFx");
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/Master");


        UpdateSliders();
    }





    private void UpdateSliders()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
    }


    public void UpdateMusicVolume(float value)
    {


        musicBus.setVolume(value);

        PlayerPrefs.SetFloat("MusicVolume", value);


    }


    public void UpdateSoundFxVolume(float value)
    {


        sfxBus.setVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);


    }



    public void UpdateMasterVolume(float value)
    {

        masterBus.setVolume(value);
        PlayerPrefs.SetFloat("Master Volume", value);

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
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }





}
