﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public GameObject moveObjectToNextScene;

    public void LoadScene()
    {
        FMODUnity.RuntimeManager.LoadBank("Master");
        StartCoroutine(MoveToScene());
    }

    IEnumerator MoveToScene()
    {


                   yield return new WaitForSeconds(1);

        if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {


            //SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            SceneManager.LoadScene(sceneToLoad);
            //For some reason, the Additive scene loading was messing with the UI of the next scene, 
            //so I changed it, but now the object below no longer carries over.  

            Scene nextScene = SceneManager.GetSceneAt(1);

            SceneManager.MoveGameObjectToScene(moveObjectToNextScene, nextScene);





            yield return null;
        }
        else
        {
            StartCoroutine(MoveToScene());
        }

        SceneManager.UnloadSceneAsync("CharacterCreation");
    }

}
