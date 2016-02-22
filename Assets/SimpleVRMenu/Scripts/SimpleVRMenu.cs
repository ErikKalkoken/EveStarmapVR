using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Main class for managing the VR Menu. 
/// Note: Please attach ths script to the PlayerController
/// </summary>
public class SimpleVRMenu : MonoBehaviour 
{
	// Public variables
	public Vector3 guiPosition      = new Vector3(0f, 0f, 1f);
	public float   guiSize          = 1f;
	public bool    useCurvedSurface = true;
	public GUISkin skin;	// current Skin to use

	public Transform rightEye;	// anchor to camera
	
	// Private variables
	private GameObject    guiRenderPlane    = null;
	private RenderTexture guiRenderTexture  = null;
	
	protected bool menuActive = false;

	// private OVRPlayerController ovrcontroler;
	private PlayerController plycontroller;
	private CustomMenu customMenu;
	private Component mainScript;
	
	private bool keypadPressed = false;
	private bool noOvrMode = false;

	// Public methods
	public void Awake ()
	{
		plycontroller = this.GetComponentInParent<PlayerController> ();	// Use alternative player controlle if OVR is not present
		noOvrMode = false;

		if ( plycontroller == null )
		{
			throw new NullReferenceException ("Can't find any player controller to connect too");
		}
								
		// create the render plane
		if (useCurvedSurface) 
		{
			guiRenderPlane = Instantiate (Resources.Load ("VRGUICurvedSurface")) as GameObject;
		} 
		else 
		{
			guiRenderPlane = Instantiate (Resources.Load ("VRGUIFlatSurface")) as GameObject;
		}
		
		// position the render plane
		guiRenderPlane.transform.parent = rightEye;
		guiRenderPlane.transform.localPosition = guiPosition;
		guiRenderPlane.transform.localRotation = Quaternion.Euler (0f, 180f, 0f);
		guiRenderPlane.transform.localScale = new Vector3 (guiSize * 1.2f, guiSize, guiSize);
		
		// create the render texture
		guiRenderTexture = new RenderTexture (Screen.width, Screen.height, 24);
		
		// assign the render texture to the render plane
		guiRenderPlane.GetComponent<Renderer>().material.mainTexture = guiRenderTexture;
		DeactivateMenu ();
	}

	void Start()
	{
	
	}
	
	public void Initialize (Component iMainScript, CustomMenu iCustomMenu) 
	{
		// Errorhandling
		if (iCustomMenu == null)
			throw new System.NullReferenceException ("Undefined iCustomMenu object in Initialize");

		// Errorhandling
		if (iMainScript == null)
			throw new System.NullReferenceException ("Undefined iMainScript object in Initialize");
		
		customMenu = iCustomMenu;
		mainScript = iMainScript;
		
	}


	// Private & Protected methods
	protected void ActivateMenu()
	{
		if (guiRenderPlane != null)
		{
			guiRenderPlane.SetActive(true);
			menuActive = true;
		}
	}
	
	protected void DeactivateMenu()
	{
		if (guiRenderPlane != null)
		{
			guiRenderPlane.SetActive(false);
			menuActive = false;
		}
	}
	
	protected void OnGUI()
	{
		
		// save current render texture
		RenderTexture tempRenderTexture = RenderTexture.active; 
		
		// set the render texture to render the GUI onto
		if (Event.current.type == EventType.Repaint)
		{			
			RenderTexture.active = guiRenderTexture;
			GL.Clear (false, true, new Color (0.0f, 0.0f, 0.0f, 0.0f));
		}
		
		// draw the Menu
		if ( menuActive && (customMenu != null) )
		{
	
			/*
			// Display Debug Info (optional)
			GUIStyle debugstyle = new GUIStyle( skin.label);
			debugstyle.normal.textColor = MyColor.gray;
			debugstyle.alignment = TextAnchor.LowerCenter;
			Vector3 currentPos = ovrcontroler.transform.position;
			GUILayout.Label ("Current Pos is: x=" + Math.Round(currentPos.x,2) + " y=" + Math.Round(currentPos.y,2) + " z=" + Math.Round(currentPos.z,2) , debugstyle);
			*/
			customMenu.DrawMenu (skin, noOvrMode);
		}
		
		if (Event.current.type == EventType.Repaint)
		{	
			// restore the previous render texture
			RenderTexture.active = tempRenderTexture;
		}
		
		
	}


	void Update()
	{
		bool GamepadButtonUp = false;
		bool GamepadButtonDown = false;
		bool GamepadButtonLeft = false;
		bool GamepadButtonRight = false;

		if (Input.GetAxis ("DPadV") < 0) GamepadButtonUp = true;
		if (Input.GetAxis ("DPadV") > 0) GamepadButtonDown = true;

		if (Input.GetAxis ("DPadH") > 0) GamepadButtonLeft = true;
		if (Input.GetAxis ("DPadH") < 0) GamepadButtonRight = true;

		// keypad needs to be set to neutral before a new key input is accepted
		if ( !GamepadButtonUp && !GamepadButtonDown && !GamepadButtonLeft && !GamepadButtonRight )
		{
			keypadPressed = false;		
		}
				
		if (menuActive && customMenu!=null) 
		{
		  
			if ( !keypadPressed && GamepadButtonUp )
			{
				keypadPressed = true;
				customMenu.PickItemAbove();
			}
			
			if ( !keypadPressed && GamepadButtonDown )
			{
				keypadPressed = true;
				customMenu.PickItemBelow();
			}
			
			if ( !keypadPressed && GamepadButtonLeft )
			{
				keypadPressed = true;
				customMenu.PickOptionLeft();
			}
			
			if ( !keypadPressed && GamepadButtonRight )
			{
				keypadPressed = true;
				customMenu.PickOptionRight();
			}
		 
			
			if ( Input.GetKeyDown(KeyCode.UpArrow) )
			{
				customMenu.PickItemAbove();
			}
			
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{		
				customMenu.PickItemBelow();
			}
			
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				customMenu.PickOptionLeft();
			}
			
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				customMenu.PickOptionRight();
			}
			
			if ( Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown (KeyCode.JoystickButton0) )
			{
				customMenu.ChooseOption();
				// Call method on main script to process changes Options
				mainScript.SendMessage ("OnSimpleVRMenuOptionChange");		
				
			}
		}

				
		// toggle the menu on/off
		if ( Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown (KeyCode.JoystickButton7) || (menuActive && Input.GetKeyDown (KeyCode.JoystickButton1)  ))
		{
			// Activate menu only it is contains at least one item
			if (!menuActive && (customMenu!=null) && (customMenu.Count > 0) )
			{
				this.ActivateMenu();
				plycontroller.SetHaltUpdateMovement(true);
			}
			else
			{
				this.DeactivateMenu();
				plycontroller.SetHaltUpdateMovement(false);
				
			}
		}


	}



}

