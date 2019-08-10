using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]
    private string SceneToLoad;

    public GameObject moveObjectToNextScene;

    /*public void LoadScene(string SceneToLoad)
    {
       
        SceneManager.LoadScene(SceneToLoad);

        SceneManager.MoveGameObjectToScene(moveObjectToNextScene, SceneManager.GetSceneByName(SceneToLoad));

        
    }*/

    public void LoadScene()
    {
        StartCoroutine(MoveToScene());
    }

    IEnumerator MoveToScene()
    {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);

        Scene nextScene = SceneManager.GetSceneAt(1);

        SceneManager.MoveGameObjectToScene(moveObjectToNextScene, nextScene);

        yield return null;

        SceneManager.UnloadSceneAsync(0);
    }

}
