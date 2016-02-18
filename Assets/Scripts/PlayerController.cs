using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class PlayerController : MonoBehaviour 
{	
	public float movement_speed = 3f;
	public float rotation_speed = 120f;

	private bool haltUpdateMovement = false;

	// START - Use this for initialization
	void Start () {}
	
	// UPDATE is called once per frame
	void Update () 
	{
		float boost = 0; 

		/*
		// recenter VR display
		if ( Input.GetButton ("BackButton") && VRDevice.isPresent )
		{
			InputTracking.Recenter();
		}
		*/

		bool LeftTrigger = false;
		if (Input.GetAxis ("LeftTrigger") > 0) LeftTrigger = true;

		if (!haltUpdateMovement)
		{
			if ( (Input.GetKey (KeyCode.LeftShift)) || (Input.GetKey (KeyCode.RightShift)) || (Input.GetButton ("LeftBumper")) || LeftTrigger )
			{
				boost = 2;
			}
			else
			{
				boost = 1;
			}

			// Moves player left, right, up, down. No collision.
			float x = Input.GetAxis("Horizontal") * Time.smoothDeltaTime * movement_speed * boost;
			float y = Input.GetAxis("Vertical") * Time.smoothDeltaTime * movement_speed * boost;
			float z = Input.GetAxis("RightV") * Time.smoothDeltaTime * movement_speed * boost;
			transform.Translate(x, z, y, Space.Self);
		
			float r = Input.GetAxis("RightH") * Time.smoothDeltaTime * rotation_speed * boost;			
			transform.Rotate (Vector3.up * r);			
		}		
	} 
	
	public void SetHaltUpdateMovement(bool toogle)
	{
		haltUpdateMovement = toogle;
	}
}