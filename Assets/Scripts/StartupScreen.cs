using UnityEngine;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartupScreen : MonoBehaviour {


	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{

		// Recenter VR display
		if ( Input.GetButton ("BackButton") && VRDevice.isPresent )
		{
			InputTracking.Recenter();
		}
								
			
		// Exit Application with ESC Key
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			Application.Quit();
		}	
				
		// Proceed to load screen	
		if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey (KeyCode.Return) || Input.GetKey (KeyCode.JoystickButton0)  )
		{
			SceneManager.LoadScene ("LoadScreen");
		}

	}
}
