using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadMainScreen : MonoBehaviour {

	public string NameOfMainLevel;	
	
	private bool first = true;
		
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
	
		if (first && (Time.timeSinceLevelLoad > 0.250F) ) 
		{
			first = false;
			
			SceneManager.LoadScene (NameOfMainLevel);
		}

	}
}
