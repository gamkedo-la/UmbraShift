using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoaded : MonoBehaviour
{
    // Start is called before the first frame update


    public EventInstance dummyEvent;

    void Start()
    {
        FMODUnity.RuntimeManager.LoadBank("Master");
        StartCoroutine(checkBankLoading());
    }

    IEnumerator checkBankLoading()
    {
        yield return new WaitForSeconds(1);

        if (RuntimeManager.HasBankLoaded("Master"))
        {
            Debug.Log("Bank Loaded starting Scene");
            SceneManager.LoadScene("MainMenu");

        }
        else
        {
            Debug.Log("Master bank not loaded looping");
            StartCoroutine(checkBankLoading());
        }


    }
}
