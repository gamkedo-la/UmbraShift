using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlLoader : MonoBehaviour
{

    int currentSceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 0) {
           // LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        //Identifying the current scene and moving on to the scene that directly follows
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
