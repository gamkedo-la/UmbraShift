using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

	private void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}
}
