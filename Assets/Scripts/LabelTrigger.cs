using UnityEngine;
using System.Collections;

public class LabelTrigger : MonoBehaviour {

	private bool showText = false;
	private GenerateGalaxy ggScript;

	// Use this for initialization
	void Awake () 
	{
		GameObject go = GameObject.Find ("Galaxy");
		if (go != null)
		{
		
			ggScript =  go.GetComponent <GenerateGalaxy>();	
			if (ggScript == null)
			{
				throw new System.NullReferenceException ("Can't find Script 'GenerateGalaxy' in LabelTrigger:Start() - ");
			}
		}
		else
		{
			throw new System.NullReferenceException ("Can't find game object 'Galaxy' in LabelTrigger:Start() - ");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {

			if (!showText) 
			{
					// Activate label on current system sphere

					Transform tf = transform.GetChild(0);
					
					if (tf != null)
					{
						tf.gameObject.SetActive (true);
						showText = true;
					
						// Add label to shortcut list
						TextMesh tm = tf.gameObject.GetComponent<TextMesh>();
						string name = tm.text;
						ggScript.AddLabelToShortlist(name, tf.gameObject);
					}
			}
	}

	void OnTriggerExit(Collider other) {
		
			if (showText) 
			{	
					// Deactivate label on current system sphere
			
					Transform tf = transform.GetChild (0);
					tf.gameObject.SetActive (false);
					showText = false;
					
					// Remove label from shortcut list
					TextMesh tm = tf.gameObject.GetComponent<TextMesh>();
					ggScript.RemoveLabeFromShortlist (tm.text);
			}
		}


}
