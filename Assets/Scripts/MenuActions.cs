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
	public Slider difficultySlider;
	public Text difficultyIndicator;
	public Text difficultyInfo;

	private const float MIN_DIFFICULTY = 1;
	private const float MAX_DIFFICULTY = 7;

    FMOD.Studio.Bus musicBus;
    FMOD.Studio.Bus sfxBus;
    FMOD.Studio.Bus masterBus;

    private void Awake()
    {
        
        StartCoroutine(ConfigureAudio());
		if (difficultyIndicator && difficultySlider) { difficultyIndicator.text = difficultySlider.value.ToString(); }
		if (difficultyInfo) { difficultyInfo.text = ""; }
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

	public void UpdateDifficulty(float diff)
	{
		Debug.Log("Setting Game Difficulty to " + diff);
		diff = Mathf.Clamp(diff, MIN_DIFFICULTY, MAX_DIFFICULTY);
		if (difficultyIndicator) { difficultyIndicator.text = diff.ToString(); }
		PlayerPrefs.SetFloat("Difficulty", diff);
		string redMessage = "";
		string greenMessage = "";
		if (diff == 1) { greenMessage = greenMessage + "\n Player base accuracy: +6%"; }
		if (diff == 2) { greenMessage = greenMessage + "\n Player base accuracy: +3%"; }
		if (diff == 4) { redMessage = redMessage + "\n Player base accuracy: -3%"; }
		if (diff == 5) { redMessage = redMessage + "\n Player base accuracy: -6%"; }
		if (diff == 6) { redMessage = redMessage + "\n Player base accuracy: -9%"; }
		if (diff == 7) { redMessage = redMessage + "\n Player base accuracy: -12%"; }
		if (diff == 1) { greenMessage = greenMessage + "\n Player hitpoints: +4 hp"; }
		if (diff == 2) { greenMessage = greenMessage + "\n Player hitpoints: +2 hp"; }
		if (diff == 4) { redMessage = redMessage + "\n Player hitpoints: -2 hp"; }
		if (diff == 5) { redMessage = redMessage + "\n Player hitpoints: -4 hp"; }
		if (diff == 6) { redMessage = redMessage + "\n Player hitpoints: -6 hp"; }
		if (diff == 7) { redMessage = redMessage + "\n Player hitpoints: -8 hp"; }
		if (diff == 1) { greenMessage = greenMessage + "\n Player skill points: +1"; }
		if (diff == 4 || diff==5) { redMessage = redMessage + "\n Player skill points: -1"; }
		if (diff == 6 || diff == 7) { redMessage = redMessage + "\n Player skill points: -2"; }
		if (diff == 1) { greenMessage = greenMessage + "\n Minimum rating of enemy Shooting skill: -1"; }
		if (diff == 4 || diff == 5) { redMessage = redMessage + "\n Minimum rating of enemy Shooting skill: +1"; }
		if (diff == 6 || diff == 7) { redMessage = redMessage + "\n Minimum rating of enemy Shooting skill: +2"; }
		if (diff == 1) { greenMessage = greenMessage + "\n Minimum rating of other enemy skills: -1"; }
		if (diff == 7) { redMessage = redMessage + "\n Minimum rating of other enemy skills: +1"; }
		if (diff == 1) { greenMessage = greenMessage + "\n Maximum rating of enemy skills: -1"; }
		if (diff == 4 || diff == 5) { redMessage = redMessage + "\n Maximum rating of enemy skills +1"; }
		if (diff == 6 || diff == 7) { redMessage = redMessage + "\n Maximum rating of enemy skills +2"; }
		if (diff == 1) { greenMessage = greenMessage + "\n Enemy base accuracy: -10%"; }
		if (diff == 2) { greenMessage = greenMessage + "\n Enemy base accuracy: -5%"; }
		if (diff == 4) { redMessage = redMessage + "\n Enemy base accuracy: +5%"; }
		if (diff == 5) { redMessage = redMessage + "\n Enemy base accuracy: +10%"; }
		if (diff == 6) { redMessage = redMessage + "\n Enemy base accuracy: +15%"; }
		if (diff == 7) { redMessage = redMessage + "\n Enemy base accuracy: +20%"; }
		if (diff == 1) { greenMessage = greenMessage + "\n One less guard in warehouse"; }
		if (diff == 2) { greenMessage = greenMessage + "\n Two less guards in warehouse"; }
		if (diff == 4) { redMessage = redMessage + "\n One additional guard in warehouse"; }
		if (diff == 5 || diff==6) { redMessage = redMessage + "\n Two additional guards in warehouse"; }
		if (diff == 7) { redMessage = redMessage + "\n Three additional guards in warehouse"; }
		if (diff == 6 || diff == 7) { redMessage = redMessage + "\n One additional cyberhound in warehouse"; }
		if (diff == 3) { greenMessage = greenMessage + "\n (default settings)"; }
		if (difficultyInfo)
		{
			if (diff == 1 || diff == 2) { difficultyInfo.color = Color.black; }
			else { difficultyInfo.color = Color.black; } 
			difficultyInfo.text = greenMessage + redMessage;
		}
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
