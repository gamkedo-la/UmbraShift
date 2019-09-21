using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlLoader : MonoBehaviour
{
	[Header("Scene Indexes")]
	[SerializeField] int mainMenu;
	[SerializeField] int charGeneration;
	[SerializeField] int barScene;
	
	int currentSceneIndex;

	// Start is called before the first frame update
	void Start()
	{
		currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

		if (currentSceneIndex == 0) {
			// LoadNextScene();
		}
	}



    IEnumerator ChangeScene(int buildID)
    {
        yield return new WaitForSeconds(1);

        if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {
            SceneManager.LoadScene(buildID);

        }
        else
        {
            Debug.Log("Master bank not loaded looping");
            StartCoroutine(ChangeScene(buildID));
        }


    }





    public void LoadNextScene()
    {
        
        //Identifying the current scene and moving on to the scene that directly follows
        SceneManager.LoadScene(currentSceneIndex + 1);
	}

	public void LoadCharGenScene()
    {
        FMODUnity.RuntimeManager.LoadBank("Master");
        //SceneManager.LoadScene(charGeneration);
        StartCoroutine(ChangeScene(charGeneration));
	}

	public void LoadBarScene()
    {
        FMODUnity.RuntimeManager.LoadBank("Master");
        //SceneManager.LoadScene(barScene);
        StartCoroutine(ChangeScene(barScene));
    }

	
}
