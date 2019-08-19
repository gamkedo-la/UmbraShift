using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenActions : MonoBehaviour
{

    public Slider masterVolumeSlider;
    public GameObject pauseMenuPanel;

    public void UpdateMasterVolume()
    {

        


        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Master Volume", masterVolumeSlider.value);
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
