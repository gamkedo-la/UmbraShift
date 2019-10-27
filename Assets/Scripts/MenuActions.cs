using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuActions : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject configMenu;
    public GameObject creditsMenu;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;


    FMOD.Studio.Bus musicBus;
    FMOD.Studio.Bus sfxBus;
    FMOD.Studio.Bus masterBus;

    private void Awake()
    {
        
        StartCoroutine(ConfigureAudio());

    }

    IEnumerator ConfigureAudio()
    {
        yield return new WaitForSeconds(1);

        if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1F);
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1F);
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1F);


            musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
            sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/SoundFx");
            masterBus = FMODUnity.RuntimeManager.GetBus("bus:/Master");


            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);



            UpdateMasterVolume(masterVolume);
            UpdateMusicVolume(musicVolume);
            UpdateSoundFxVolume(sfxVolume);

        }
        else
        {
            StartCoroutine(ConfigureAudio());
        }


    }



   

    






    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        configMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void ShowConfigMenu()
    {
        mainMenu.SetActive(false);
        configMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void ShowCreditsMenu()
    {
        mainMenu.SetActive(false);
        configMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }


    public void UpdateMusicVolume(float value)
    {
        Debug.Log("Setting Music Volume");

        musicBus.setVolume(value);

        musicBus.setVolume(value);


    }


    public void UpdateSoundFxVolume(float value)
    {
        Debug.Log("Setting SFX Volume");

        sfxBus.setVolume(value);
       


    }



    public void UpdateMasterVolume(float value)
    {

        Debug.Log("Setting Master Volume");

        masterBus.setVolume(value);
        PlayerPrefs.SetFloat("Master Volume", value);

    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
