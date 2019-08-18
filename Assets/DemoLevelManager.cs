using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoLevelManager : MonoBehaviour
{
	[SerializeField] private string[] sceneNames;
	[SerializeField] private Canvas canvas;
	private SceneManager sceneManager;
	private int levelToPlay = 0;

	private void Awake()
	{
		DontDestroyOnLoad(canvas.gameObject);
	}

	public void AdvanceLevel()
	{
		levelToPlay = levelToPlay + 1;
		if (levelToPlay > sceneNames.Length - 1) { SceneManager.LoadScene(sceneNames[0]); }
		else { SceneManager.LoadScene(sceneNames[levelToPlay]); }
	}

}
