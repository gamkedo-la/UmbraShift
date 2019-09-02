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

	public void LoadNextScene()
	{
		//Identifying the current scene and moving on to the scene that directly follows
		SceneManager.LoadScene(currentSceneIndex + 1);
	}

	public void LoadCharGenScene()
	{
		SceneManager.LoadScene(charGeneration);
	}

	public void LoadBarScene()
	{
		SceneManager.LoadScene(barScene);
	}

	
}
