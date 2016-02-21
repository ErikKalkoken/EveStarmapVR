/*
 * GenerateGalaxy.cs
 *
 * Copyright (C) 2015 Erik Kalkoken
 *
 * Generates all galaxy 3D objects in space at startup based on CCP's map data
 *
 * HISTORY
 * 20-FEB-2016 v0.7.0 New: Shows current jumps/kills for each system
 * 18-FEB-2016 v0.6.1 Performance improvements by introducing static batching for all solar systen spheres
 * 13-DEC-2015 v0.1.0 Initial version
 *
**/

using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GenerateGalaxy : MonoBehaviour
{

		// Interface to editor
		public TextAsset mapRegionsCSV;
		public TextAsset mapSolarSystemsCSV;
		public TextAsset mapSolarSystemJumpsCSV;
		public TextAsset chrFactionsCSV;
		public bool generateGalaxy = true;
		public bool generateJumpLines = true;
		public Shader shader;
		public Font fontReg;
		public Material fontRegMat;
		public GameObject spherePrefab;
		public Transform playerController;
		public float lineSize = 0.004f;	// Width of "line" in space drawn for jump connections
		public float textVisibilityRange = 350f; 	// distance from player where labels are visible
		public float sphereSize = 0.03f;
		
		// Store data about all solor systems
		class SolarSystem
		{
				public int RegionID { get; set; }
				public int ConstellationID  { get; set; }
				public int SolarSystemID { get; set; }
				public string SolarSystemName { get; set; }
				public float Luminosity { get; set; }
				public bool IsHub { get; set; }
				public bool IsRegional { get; set; }
				public int FactionID { get; set; }
				public Vector3 Position { get; set; }	// coordinates are stored after downscaling is applied
				public float Sec { get; set; }
				public Transform Trf  { get; set; }
				public Material colorMat  { get; set; }
				public int kills { get; set; }
				public int jumps { get; set; }
		};


		// Store data about all regions
		class Region
		{
				public int RegionID { get; set; }
				public string RegionName  { get; set; }
				public Vector3 Position  { get; set; }
				public Color Col  { get; set; }
				public int FactionID  { get; set; }
				public GameObject Go  { get; set; }
				public GameObject GoLabel  { get; set; }
				public GameObject JumpsGo  { get; set; }
				public Mesh JumpsMesh  { get; set; }
				public Material JumpsMat  { get; set; }
				public Material SystemMat  { get; set; }
		};
		
		class Faction
		{
				public int FactionID { get; set; }
				public string FactionName  { get; set; }
				public Color Col  { get; set; }
				public Vector3 Position { get; set; }	// coordinates are stored after downscaling is applied
				public GameObject GoLabel  { get; set; }
		
		}
		
		// Private objects and variables
		private Dictionary<int,SolarSystem> solarSystems;
		private Dictionary<int,Region> regions;
		private Dictionary<int, Faction> factions;
		private SimpleVRMenu simpleVRMenu;	// reference to menu
		private CustomMenu customMenu;
		private GameObject ovrPlayerController;
		private float galaxyCoordinateScale;
		private Dictionary<string,GameObject> labelShortlist;	// Contains list of all visible labels to enable real-time rotation adjustment
		private int updateCounter = 1;		// counter to manage number of calls to rotation adjustments to labels
		private const int updateLabelRotationFrameCount = 3; // Text rotation will be updated every 3rd frame - performance tweak

		private Dictionary<int,int> systemTopKills;	// sorted lookup table for top kills with systemId, kills
		private Dictionary<int,int> systemTopJumps; // sorted lookup table for top jumps with systemId, jumps
		enum SystemStatsType { none, kills, jumps };
		private GameObject highlightSpehres = null;

		// Faction IDs
		private const int factionIdMin = 500000;
		private const int factionIdMax = 500020;

		// Predefined materials to improve performance
		private Material jumpsBasisMat;				// Basis material for all other materials used for jumps
		private Material systemBasisMat;			// Basis material for all other materials used for solar systems
		private Material systemSuperHighSecMat;
		private Material systemHighSecMat;
		private Material system05SecMat;
		private Material systemLowSecMat;
		private Material systemNullSecMat;
		private Material systemDetailsMat;
		private Dictionary<int,Material> systemsFactionMat;
		private Dictionary<int,Material> jumpsFactionMat;
		private Dictionary<char,Material> solarColorMat;
		private Material fontRegPrvMat;
//		private Material fontBoldPrvMat;

		//////////////////////////////////////////////////////
		// Public methods
		//////////////////////////////////////////////////////

		GenerateGalaxy()
		{
			
		}

		public void Start ()
		{
				labelShortlist = new Dictionary<string,GameObject> ();
			
				if (generateGalaxy == false) {
						return;
				}
				
				// Errorhandling for public objects
				if (
						(mapRegionsCSV == null) 
						|| (mapRegionsCSV == null) 
						|| (mapSolarSystemsCSV == null) 
						|| (mapSolarSystemJumpsCSV == null) 
				) {
						Debug.LogError ("Textfile not defined in public objects");
						generateGalaxy = false;
						Application.Quit ();
						return;
				}
				
				// Initialize global variables
				galaxyCoordinateScale = (float)Math.Pow (10, 16) / 2f; // Factor to scale universe coordinates down to feasable numbers

				// Initialize options menu
				InitializeMenu ();
		
				// Initialize predefined materials
				InitializeMaterials ();	
																
				GenerateRegions ();
				GenerateSolarSystems ();
				RetrieveSystemDetails ();
				GenerateJumps ();
				GenerateFactions ();

				// Paint solar systems with default color schema
				PaintSystemsSec ();
				ToogleDisplayFactionLabels (false);
			
				// Universe generation complete - starting background music
				if (transform.gameObject.GetComponent<AudioSource>() != null) {
						transform.gameObject.GetComponent<AudioSource>().Play ();
				}
		
				// Setting player in start position
				if (playerController != null) {
						playerController.transform.position = new Vector3 (-24.55f, 34.19f, 24.73f);
				}
		}

		public void Update ()
		{
				// Execute rotation adjustment of labels only every nth frame to save drawcalls
				if (updateCounter == updateLabelRotationFrameCount) {	
						updateCounter = 1;	// reset counter
						
						// Adjust rotation of active labels so they are readible from the player camera
						foreach (GameObject goLabel in labelShortlist.Values) {		
								goLabel.transform.LookAt (playerController.transform, playerController.transform.up);
								goLabel.transform.Rotate (0, 180, 0);		
						}		
				}
				updateCounter++;
		
				// Exit Application with ESC Key
				if (Input.GetKeyDown (KeyCode.Escape)) {
						Application.Quit ();
				}
				
		}

		/// <summary>
		/// Is called when the menu is closed to process the results
		/// </summary>
		public void OnSimpleVRMenuOptionChange ()
		{
				List<int> currentOpt = customMenu.GetCurrentOptions ();
				List<int> defaultOpt = customMenu.GetDefaultOptions ();
				
		
				// Check which option has changed - if any	
				bool optionsChanged = false;
				int changedOptionIdx = -1;	
				for (int i=0; i<currentOpt.Count; i++) {
						if (currentOpt [i] != defaultOpt [i]) {
								optionsChanged = true;
								changedOptionIdx = i;
								break; 
						}
			
				}	
		
				// Apply effect of changed option		
				if (optionsChanged) {
						switch (changedOptionIdx) {
						case 0:		// color scheme

								switch (currentOpt [changedOptionIdx]) {
								case 0:
										PaintSystemsSec ();
										break;
				
								case 1:
										PaintSystemsRegional ();
										break;
					
								case 2:
										PaintSystemsFaction ();
										break;
					
								case 3:
										PaintSystemsSolarColor ();
										break;
					
								case 4:
//										PaintSystemsRandom ();
										break;
								}
								break;
					
						case 1: // Color of Jump Connections
								switch (currentOpt [changedOptionIdx]) {				
								case 0: // neutral
										PaintJumpsNeutral ();
										break;
						
								case 1: // Regions
										PaintJumpsRegionColors ();
										break;			
							
								case 2: // Factions
										PaintJumpsFactionColors ();
										break;								
								}
								break;
				
				
						case 2: // Display region names
								if (currentOpt [changedOptionIdx] == 0) {
										ToogleDisplayRegionLabels (false);
								} else {
										ToogleDisplayRegionLabels (true);
								}			
								break;

						case 3: // Display faction names
								if (currentOpt [changedOptionIdx] == 0) {
										ToogleDisplayFactionLabels (false);
								} else {
										ToogleDisplayFactionLabels (true);
								}			
								break;
				
						case 4: // Show system details
								switch (currentOpt [changedOptionIdx]) {				
								case 0: // none
										ShowSystemDetails (SystemStatsType.none);
										break;
						
								case 1: // kills
										ShowSystemDetails (SystemStatsType.kills);
										break;			
							
								case 2: // jumps
										ShowSystemDetails (SystemStatsType.jumps);
										break;								
								}	
								break;
				
						case 5: // Display jump connections
								if (currentOpt [changedOptionIdx] == 0) {
										ToogleDisplayJumps (false);
								} else {
										ToogleDisplayJumps (true);
								}			
								break;

						case 6: // Turn music on/off
								if (currentOpt [changedOptionIdx] == 0) {
										ToogleBackgroundMusic (false);
								} else {
										ToogleBackgroundMusic (true);
								}			
								break;
/*
				case 4: // Gamepad setup
					if (currentOpt[changedOptionIdx] == 1)
					{
						OVRPlayerController script = ovrPlayerController.GetComponent("OVRPlayerController") as OVRPlayerController;
						script.FlightControls = true;
					}
					else
					{
						OVRPlayerController	script = ovrPlayerController.GetComponent("OVRPlayerController") as OVRPlayerController;
						script.FlightControls = false;
					}			
					break;
*/				
				
						default:
								break;
			
						}
						customMenu.ResetDefaultOptions ();
			
				}
		}		
	
		/// <summary>
		/// Adds one label to the shortlist.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="goLabel">Go label.</param>
		public void AddLabelToShortlist (string name, GameObject goLabel)
		{
				labelShortlist.Add (name, goLabel);	
		}	
	
		/// <summary>
		/// Removes given label from the shortlist.
		/// </summary>
		/// <param name="name">Name.</param>
		public void RemoveLabeFromShortlist (string name)
		{
				labelShortlist.Remove (name);	
		}	
	
	
		//////////////////////////////////////////////////////
		// Private methods
		//////////////////////////////////////////////////////
	
		/// <summary>
		/// Initializes the options menu.
		/// </summary>
		private void InitializeMenu ()
		{
				// Define and initialize menu
				if (playerController != null) {
						// Connect to Player Controller to find SimpleVRMenu
						simpleVRMenu = playerController.GetComponent ("SimpleVRMenu") as SimpleVRMenu;
						customMenu = new CustomMenu ();
			
						if (simpleVRMenu != null) {
								customMenu.AddItem ("Solar system colors", new string[] {
										"Security Status",
										"Regions",
										"Factions", "Solar Color"
								}, 0);
								customMenu.AddItem ("Jump connection colors", new string[] {
										"Neutral",
										"Regions",
										"Factions"
								}, 0);
								customMenu.AddItem ("Show region names", new string[] {"no", "yes"}, 1);
								customMenu.AddItem ("Show faction names", new string[] {"no", "yes"}, 0);
								customMenu.AddItem ("Show system details", new string[] {"none", "kills", "jumps"}, 0);
								customMenu.AddItem ("Show jump connections", new string[] {"no", "yes"}, 1);
								customMenu.AddItem ("Play background music", new string[] {"no", "yes"}, 1);
								//		customMenu.AddItem ("Gamepad setup", new string[] {"standard", "flight control"}, 0);
				
								simpleVRMenu.Initialize (this, customMenu);
				
						}
				} else {	
						Debug.LogWarning ("Can't find PlayerController. Is it attached ??");
				}
		}
	
		/// <summary>
		/// Generates the regions.
		/// </summary>
		private void GenerateRegions ()
		{	
				// Load region data from file
				string[,] regiongrid = CSVReader.SplitCsvGrid (mapRegionsCSV.text); // loading map of star systems
		
				// Initialize region array to store data
				regions = new Dictionary<int,Region> ();
				int regionrows = regiongrid.GetUpperBound (1);
//				int t = 0; // Counter for lookupRegions
		
				// Process and store region data. Create region game objects.
				for (int i=1; i<regionrows; i++) {
						int regionID = Convert.ToInt32 (regiongrid [0, i]); // get region ID
			
						if ((regionID > 0) && (regionID < 11000000)) {
								string regionName = regiongrid [1, i];
								float x = Convert.ToSingle (regiongrid [2, i]) / galaxyCoordinateScale;
								float y = Convert.ToSingle (regiongrid [3, i]) / galaxyCoordinateScale;
								float z = Convert.ToSingle (regiongrid [4, i]) / galaxyCoordinateScale;		
								Vector3 position = new Vector3 (x, y, z);
								int factionId = ConvertIDFromString (regiongrid [11, i]); // get faction ID
								factionId = (factionId >= factionIdMin) && (factionId <= factionIdMax) ? factionId : 500000;	// Overwrite ID with 500.000 if unvalid
								Color color = GenerateRandomRegionColor ();
								
								Region region = new Region ();												
								region.RegionID = regionID;
								region.RegionName = regionName;
								region.Position = position;
								region.FactionID = factionId;
								region.Col = color;
								region.SystemMat = new Material (systemBasisMat);
								region.SystemMat.color = color;
								region.JumpsMat = new Material (jumpsBasisMat);
								region.JumpsMat.color = color;		
								region.Go = new GameObject (regionName);
								regions.Add (regionID, region);
						}
				}
		
				// Generate region labels in space
				foreach (Region region in regions.Values) {
						string name = region.RegionName;
						Vector3 position = region.Position;
									
						GameObject go = new GameObject ();
						TextMesh textMesh = go.AddComponent <TextMesh>() as TextMesh;
			
						go.name = "label_" + name;
						go.transform.position = position;
						go.transform.localScale = new Vector3 (0.06F, 0.06F, 0.06F);
						go.transform.rotation = Quaternion.Euler (new Vector3 (90, 0, 0));
			
						textMesh.font = fontReg;
						textMesh.text = name;
						textMesh.GetComponent<Renderer>().material = fontRegPrvMat;
						textMesh.fontSize = 120;
			
						go.transform.parent = region.Go.transform; 			// Newly generated label is child of region object
						region.GoLabel = go;									// Save reference to label gameobject for fast access later
				}
			
		}
	
		/// <summary>
		/// Generates the solar systems.
		/// </summary>
		private void GenerateSolarSystems ()
		{
				// Load solarSystems from file
				string[,] sysgrid = CSVReader.SplitCsvGrid (mapSolarSystemsCSV.text); // loading map of star systems
				int sysrows = sysgrid.GetUpperBound (1);
	
				// Create new array to store systems data
				solarSystems = new Dictionary<int, SolarSystem> ();
				//		int id = 0;
		
				// Iterate through all star systems
		
				// Initialize & Parameters
				//		Color solorSystemColor = MyColor.gray;
		
				// Shader solorSystemShader = Shader.Find("Self-Illumin/VertexLit");
		
		
				for (int i = 1; i < sysrows; i++) {
						// get data from csv file
						int id = Convert.ToInt32 (sysgrid [2, i]); // get system ID
			
						if ((id > 0) && (id < 31000000)) { // exclude WH systems
				
								// get data from csv file
								int regionId = Convert.ToInt32 (sysgrid [0, i]); // get region ID
								int constellation = Convert.ToInt32 (sysgrid [1, i]); // get region ID				
								string name = sysgrid [3, i];
								float x = Convert.ToSingle (sysgrid [4, i]) / galaxyCoordinateScale;
								float y = Convert.ToSingle (sysgrid [5, i]) / galaxyCoordinateScale;
								float z = Convert.ToSingle (sysgrid [6, i]) / galaxyCoordinateScale;
								Vector3 position = new Vector3 (x, y, z);
								int tmp = Convert.ToInt16 (sysgrid [17, i]);
								bool isHub = (tmp == 0) ? false : true;
								tmp = Convert.ToInt16 (sysgrid [19, i]);
								bool isRegional = (tmp == 0) ? false : true;
								float sec = (float)Math.Round (Convert.ToDouble (sysgrid [21, i]), 1);
								float luminosity = Convert.ToSingle (sysgrid [13, i]);
								int factionID = ConvertIDFromString (sysgrid [22, i]); // get faction ID
								factionID = (factionID >= factionIdMin) && (factionID <= factionIdMax) ? factionID : 500000;	// Overwrite ID with 500.000 if unvalid
								
								Material colorMat;
								string classtxt = sysgrid [26, i];
								char solarClass = classtxt [0];
								if (solarColorMat.ContainsKey (solarClass)) {
										colorMat = solarColorMat [classtxt [0]];
								} else {
										colorMat = solarColorMat ['G'];
								}
								
								// Save system info in hashtable
								SolarSystem system = new SolarSystem ();
																
								system.RegionID = regionId;
								system.ConstellationID = constellation;
								system.SolarSystemID = id;
								system.SolarSystemName = name;
								system.Position = position;
								system.IsHub = isHub;
								system.IsRegional = isRegional;
								system.Sec = sec;
								system.Luminosity = luminosity;
								system.FactionID = factionID;
								system.colorMat = colorMat;
								
				
								// Draw one star system
								GameObject sphere = GameObject.Instantiate (spherePrefab) as GameObject;
				
								// GameObject name at runtime is name of system
								sphere.name = name;																
								sphere.transform.GetComponent<Renderer>().material = systemBasisMat;
																			
								// anchor system to runtime parent and store object in array for further usage (e.g. to rest all colors)
								sphere.transform.parent = regions [regionId].Go.transform;
								system.Trf = sphere.transform;
				
								// Initialize transform and renderer
								sphere.transform.position = position;
								sphere.transform.localScale = new Vector3 (sphereSize, sphereSize, sphereSize);
				
								// Define collider for text display
								SphereCollider sc = sphere.GetComponent<Collider>() as SphereCollider;
								sc.isTrigger = true;					// Use Collider as Triger to be able to move around freely
								sc.radius = textVisibilityRange;	
								sphere.AddComponent<LabelTrigger> ();				// adding custom script to enable automatic activation / deactivation of labels		
							
								solarSystems.Add (id, system);
										
								// Add name label to star system
								GameObject go = new GameObject ();
								TextMesh textMesh = go.AddComponent <TextMesh>() as TextMesh;
				
								go.name = "label_" + name;
								go.transform.position = new Vector3 (x, y, z);
								const float fontSize = 0.025f;
								go.transform.localScale = new Vector3 (fontSize, fontSize, fontSize);
								go.transform.Rotate (new Vector3 (90, 0, 0));
				
								textMesh.font = fontReg;
								textMesh.text = name;
								textMesh.GetComponent<Renderer>().material = fontRegPrvMat;
								textMesh.fontSize = 64;
				
								go.SetActive (false);								// deactive for now
				
								go.transform.parent = sphere.transform; 			// Newly generated label is child of current system object
								sphere.isStatic = true;								// set as static to improvce performance
				
						}
			
				}
				

				// enforce static batching for region trees (only works with Unity Pro though
				foreach (Region region in regions.Values) 
				{
					StaticBatchingUtility.Combine (region.Go); // Optimze tree of star systems for performance
				}					
		}
	
		/// <summary>
		/// Generates the jump connections.
		/// </summary>
		private void GenerateJumps ()
		{
		
				// JUMP CONNECTIONS
				// ----------------------
		
				if (generateJumpLines) {
			
						// Read jump lines from file
			
						string[,] jumpgrid = CSVReader.SplitCsvGrid (mapSolarSystemJumpsCSV.text); // loading map of jump connections
						int jumprows = jumpgrid.GetUpperBound (1);
						
						// Create one empty gameobjects for meshes in each region
						foreach (Region region in regions.Values) {
								GameObject jumpsGo = new GameObject ();
												
								jumpsGo.name = "Jump Connections";
								jumpsGo.transform.parent = region.Go.transform;
								Mesh jumpMesh = new Mesh ();
								jumpsGo.AddComponent<MeshFilter> ().mesh = jumpMesh;
								region.JumpsMesh = jumpMesh;
								jumpsGo.AddComponent<MeshRenderer> ();
								jumpsGo.GetComponent<Renderer>().material = jumpsBasisMat;
							
								region.JumpsGo = jumpsGo;
						}
										
						// Iterate through all jump connections
												
						for (int i = 1; i < jumprows; i++) {
								int sysFromId = Convert.ToInt32 (jumpgrid [2, i]);
								int sysToId = Convert.ToInt32 (jumpgrid [3, i]);
								
								if (solarSystems.ContainsKey (sysFromId) && solarSystems.ContainsKey (sysToId)) {
										Vector3 from = solarSystems [sysFromId].Position;
										Vector3 to = solarSystems [sysToId].Position;

										// Attach to region based on fromID
										int regionId = solarSystems [sysFromId].RegionID;
										if (regions.ContainsKey (regionId)) {
												Mesh currentMesh = regions [regionId].JumpsMesh;
									
												// Create two lines for each jump connection for better visibility					
												AddLine (currentMesh, MakeQuad (from, to, lineSize), false);
												AddLine (currentMesh, MakeQuad2 (from, to, lineSize), false);
										}
								}
						}
			
						// Update texture coordinates and normals for all meshes
						foreach (Region region in regions.Values) {
								UpdateMeshUvs (region.JumpsMesh);
								region.JumpsMesh.RecalculateNormals ();
						}
				}
		}

		/// <summary>
		/// Generates faction labels in space
		/// </summary>
		private void GenerateFactions ()
		{	
				// Load region data from file
				string[,] factiongrid = CSVReader.SplitCsvGrid (chrFactionsCSV.text); // loading map of star systems
		
				// Initialize region array to store data
				factions = new Dictionary<int,Faction> ();
		
				int rows = factiongrid.GetUpperBound (1);
	
				// Process and store region data. Create region game objects.
				for (int i=1; i<rows; i++) {
						int factionID = Convert.ToInt32 (factiongrid [0, i]); // get region ID
			
						if ((factionID >= 500000) && (factionID < 500100)) {
								string factionName = factiongrid [1, i];
								int solarSystemId = ConvertIDFromString (factiongrid [3, i]);		
								Vector3 position = solarSystems [solarSystemId].Position;
								Color color = GetFactionColorFromID (factionID);
								
								Faction faction = new Faction ();
								faction.FactionID = factionID;
								faction.FactionName = factionName;
								faction.Position = position;
								faction.Col = color;
								faction.GoLabel = new GameObject (factionName);
								factions.Add (factionID, faction);
				
						}
				}
		
				// Generate faction labels in space
				foreach (Faction faction in factions.Values) {
						string name = faction.FactionName;
						Vector3 position = faction.Position;
			
						GameObject go = new GameObject ();
						TextMesh textMesh = go.AddComponent <TextMesh>() as TextMesh;
			
						go.name = "label_" + name;
						go.transform.position = position;
						go.transform.localScale = new Vector3 (0.06F, 0.06F, 0.06F);
						go.transform.rotation = Quaternion.Euler (new Vector3 (90, 0, 0));
			
						textMesh.font = fontReg;
						textMesh.text = name;
						textMesh.GetComponent<Renderer>().material = fontRegPrvMat;
						textMesh.fontSize = 180;
			
						faction.GoLabel = go;									// Save reference to label gameobject for fast access later
				}
		
		}

		/// <summary>
		/// Retrieves current kills and jumps per system from Eve API and store them for later use
		/// </summary>
		private void RetrieveSystemDetails ()
		{
				EveApi.initHttps();

				Dictionary<int, int> systemKills = new Dictionary<int, int>();
				Dictionary<int, int> systemJumps = new Dictionary<int, int>();

				// load current system stats from EveApi
				systemJumps = EveApi.getJumps();
				systemKills = EveApi.getKills();

				// attach info to solarSystems
				foreach (SolarSystem system in solarSystems.Values) 
				{
						int solarSystemId = system.SolarSystemID;
						solarSystems[solarSystemId].kills = systemKills.ContainsKey(solarSystemId) ? systemKills[solarSystemId] : 0;
						solarSystems[solarSystemId].jumps = systemJumps.ContainsKey(solarSystemId) ? systemJumps[solarSystemId] : 0;
				}

				// store lookup tables for top systems
				int max = 50;
				systemTopKills = EveApi.getTop (systemKills, max);
				systemTopJumps = EveApi.getTop (systemJumps, max);
		}

		/// <summary>
		/// Show current kills or jumps per system from Eve API
		/// </summary>
		private void ShowSystemDetails (SystemStatsType statsType)
		{
			// remove previous highlight speheres
			if (highlightSpehres != null)
			{
				Destroy (highlightSpehres);
				highlightSpehres = null;
			}

			switch (statsType)
			{
				case SystemStatsType.kills:
					// attach info to solarSystem labels
					foreach (SolarSystem system in solarSystems.Values) 
					{
						int solarSystemId = system.SolarSystemID;

						Transform label = system.Trf.GetChild(0);
						TextMesh textMesh = label.GetComponent<TextMesh>();
						textMesh.text = system.SolarSystemName + " (" + system.kills + ")";
					}

					highlightSpehres = new GameObject("System Details");

					foreach (KeyValuePair<int,int> item in systemTopKills)
					{ 
							int solarSystemId = item.Key;

							if (solarSystems.ContainsKey(solarSystemId))
							{
								GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
								sphere.transform.position = solarSystems[solarSystemId].Position;
								float radius = 0.1f * item.Value;
								sphere.transform.localScale = new Vector3(radius, radius, radius);
								sphere.transform.GetComponent<Renderer>().material = systemDetailsMat;
								sphere.name = solarSystems[item.Key].SolarSystemName + "_kills";
								sphere.isStatic = true;								// set as static to improvce performance

								systemDetailsMat.color = new Color(1F, 0F, 0F, 0.7F);

								// store current sphere on stack for later retrieval
								sphere.transform.parent = highlightSpehres.transform;
							}
					}
					StaticBatchingUtility.Combine (highlightSpehres);
					break;

				case SystemStatsType.jumps:
					// attach info to solarSystem labels
					foreach (SolarSystem system in solarSystems.Values) 
					{
						int solarSystemId = system.SolarSystemID;

						Transform label = system.Trf.GetChild(0);
						TextMesh textMesh = label.GetComponent<TextMesh>();
						textMesh.text = system.SolarSystemName + " (" + system.jumps + ")";
					}

					highlightSpehres = new GameObject("System Details");

					foreach (KeyValuePair<int,int> item in systemTopJumps)
					{ 
							int solarSystemId = item.Key;

							if (solarSystems.ContainsKey(solarSystemId))
							{
								GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
								sphere.transform.position = solarSystems[solarSystemId].Position;
								float radius = 0.003f * item.Value;
								sphere.transform.localScale = new Vector3(radius, radius, radius);
								sphere.transform.GetComponent<Renderer>().material = systemDetailsMat;
								sphere.name = solarSystems[item.Key].SolarSystemName + "_jumps";
								sphere.isStatic = true;								// set as static to improvce performance

								systemDetailsMat.color = new Color(0F, 1F, 0F, 0.7F);

								// store current sphere on stack for later retrieval
								sphere.transform.parent = highlightSpehres.transform;
							}
					}
					StaticBatchingUtility.Combine (highlightSpehres);
					break;

				default:
					// remove sytem details from Labels
					foreach (SolarSystem system in solarSystems.Values) 
					{
						Transform label = system.Trf.GetChild(0);
						TextMesh textMesh = label.GetComponent<TextMesh>();
						textMesh.text = system.SolarSystemName;
					}
					break;
			}
		}
	
		// *******************************
		// * Support methods
		// *******************************
		
		private void InitializeMaterials ()
		{
				// Solar systems
		
				GameObject sphere = GameObject.Instantiate (spherePrefab) as GameObject;
				systemBasisMat = new Material (sphere.GetComponent<Renderer>().material);
		
				systemSuperHighSecMat = new Material (systemBasisMat);		
				systemSuperHighSecMat.color = MyColor.cyan;// green for Hig Sec 0.9+
		
				systemHighSecMat = new Material (systemBasisMat);		
				systemHighSecMat.color = MyColor.green;// green for normal Hig Sec
		
				system05SecMat = new Material (systemBasisMat);		
				system05SecMat.color = MyColor.yellow; // yellow for 0.5 system
		
				systemLowSecMat = new Material (systemBasisMat);		
				systemLowSecMat.color = MyColor.darkOrange; // Orange 
		
				systemNullSecMat = new Material (systemBasisMat);		
				systemNullSecMat.color = MyColor.crimson; // default is red for Nullsec
	
				systemsFactionMat = new Dictionary<int,Material> ();	
				for (int i=factionIdMin; i<factionIdMax+1; i++) {
						Material mat = new Material (systemBasisMat);
						mat.color = GetFactionColorFromID (i);
						systemsFactionMat.Add (i, mat);
				}
				Destroy (sphere); 
		
				// Jumps
				Shader ns = Shader.Find ("Custom/JumpConnect");
		
				if (ns == null) {
						Debug.LogWarning ("Shader for Jump Connections not found");
						ns = Shader.Find ("Self-Illumin/VertexLit");
				}		
				jumpsBasisMat = new Material (ns);
				jumpsBasisMat.color = MyColor.steelBlue;
	
				jumpsFactionMat = new Dictionary<int,Material> ();	
				for (int i=factionIdMin; i<factionIdMax+1; i++) {
						Material mat = new Material (jumpsBasisMat);
						mat.color = GetFactionColorFromID (i);
						jumpsFactionMat.Add (i, mat);
				}	
		
				// Solar Colors
				solarColorMat = new Dictionary<char, Material> ();
				CreateSolarColorMaterials (systemBasisMat);
		
				// Textmaterials and fonts
//				fontBoldPrvMat = new Material (fontBoldMat);
				fontRegPrvMat = new Material (fontRegMat);	

				// systemDetails
				systemDetailsMat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
				systemDetailsMat.color = new Color(0F, 1F, 0F, 0.7F);
				// systemDetailsMat = new Material(Shader.Find("Standard"));
				// systemDetailsMat.EnableKeyword("_ALPHATEST_ON");
		}
/*
	private void DrawLineLR (Vector3 start, Vector3 end)
	{
		GameObject jumpLine = new GameObject();
		LineRenderer lineRenderer = jumpLine.AddComponent<LineRenderer>();
		
		lineRenderer.material = new Material(Shader.Find("VertexLit"));
		lineRenderer.SetColors(Color.white, Color.white);
		lineRenderer.SetWidth(0.004F, 0.004F);
		lineRenderer.SetVertexCount(2);
		
		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, end);
		
//		jumpLine.transform.parent = AnchorJumpLines.transform; 			// Newly generated system is child of current Game Object

	}
*/	
		/// <summary>
		/// Activates or deactivates the region labels
		/// </summary>
		/// <param name="showLabels">If set to <c>true</c> show labels.</param>
		private void ToogleDisplayRegionLabels (bool showLabels)
		{
				foreach (Region region in regions.Values) {
						region.GoLabel.SetActive (showLabels);
				}		
		}

		private void ToogleDisplayJumps (bool showJumps)
		{
				foreach (Region region in regions.Values) {
						region.JumpsGo.SetActive (showJumps);
				}		
		}
		
		private void ToogleDisplayFactionLabels (bool showLabels)
		{
				foreach (Faction faction in factions.Values) {
						faction.GoLabel.SetActive (showLabels);
				}		
		}
	
		/// <summary>
		/// Paints the solar system with regional color scheme
		/// </summary>
		private void PaintSystemsRegional ()
		{
				foreach (SolarSystem sol in solarSystems.Values) {
						GameObject go = sol.Trf.gameObject;
						go.GetComponent<Renderer>().material = regions [sol.RegionID].SystemMat;
			
				}
		}

		/// <summary>
		/// Paints the solar system with "solar color" color scheme.
		/// </summary>
		private void PaintSystemsSolarColor ()
		{
				foreach (SolarSystem sol in solarSystems.Values) {
						GameObject go = sol.Trf.gameObject;
						go.GetComponent<Renderer>().material = sol.colorMat;
				}	
		}
	
		/*
		/// <summary>
		/// Paints the solar system with random color scheme
		/// </summary>
		void PaintSystemsRandom ()
		{

				/*
				for (int k=0; k < countSystems; k++) {

	
				0			GameObject go = solarSystems [k].transform.gameObject;
						go.renderer.material.color = new Color (UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
				}

		}
*/
		/// <summary>
		/// Paints the solar system with faction color scheme
		/// </summary>
		private void PaintSystemsFaction ()
		{	
				foreach (SolarSystem sol in solarSystems.Values) {
						GameObject go = sol.Trf.gameObject;
						int factionId = sol.FactionID;
						go.GetComponent<Renderer>().material = systemsFactionMat [factionId];
				}
		}
	
		/// <summary>
		/// Paints the solar system with "security status" color scheme
		/// </summary>
		private void PaintSystemsSec ()
		{
				foreach (SolarSystem sol in solarSystems.Values) {
				
						Material mat = systemNullSecMat;
						float sec = sol.Sec;

						if (sec >= 0.9F) {
								mat = systemSuperHighSecMat;
						}
			
						if ((sec >= 0.6F) && (sec < 0.9F)) {
								mat = systemHighSecMat; 
						}
			
						if ((sec >= 0.5F) && (sec < 0.6F)) {
								mat = system05SecMat; 
						}
			
						if ((sec > 0.0F) && (sec < 0.5F)) {
								mat = systemLowSecMat;  
						}

						if (sec <= 0.0F) {
								mat = systemNullSecMat;
						}
			
						GameObject go = sol.Trf.gameObject;
						go.GetComponent<Renderer>().material = mat;
				}
		
		}

		/// <summary>
		/// Paints all jump connections with a custom color
		/// </summary>
		/// <param name="c">C.</param>
		private void PaintJumpsCustomColor (Color c)
		{
				foreach (Region region in regions.Values) {
						region.JumpsGo.GetComponent<Renderer>().material.color = c;
				}
				
				
		}
	
		/// <summary>
		/// Paints all jump connections with a custom color
		/// </summary>
		/// <param name="c">C.</param>
		private void PaintJumpsNeutral ()
		{
				foreach (Region region in regions.Values) {
						region.JumpsGo.GetComponent<Renderer>().material = jumpsBasisMat;
				}
			
		}
		/// <summary>
		/// Paints the jump connections with the color of its region
		/// </summary>
		private void PaintJumpsRegionColors ()
		{	
				foreach (Region region in regions.Values) {
						region.JumpsGo.GetComponent<Renderer>().material = region.JumpsMat;
				}				
		}
	
		/// <summary>
		/// Paints the jump connections with the color of its race
		/// </summary>
		private void PaintJumpsFactionColors ()
		{	
				foreach (Region region in regions.Values) {
						int factionId = region.FactionID;
						region.JumpsGo.GetComponent<Renderer>().material = jumpsFactionMat [factionId];
				}				
		}
				
	
		/// <summary>
		/// Toogles playing of the background music.
		/// </summary>
		/// <param name="playMusic">If set to <c>true</c> play music.</param>
		private void ToogleBackgroundMusic (bool playMusic)
		{
				AudioSource music = this.GetComponent <AudioSource> ();
		
				if (music != null) {
		
						if (playMusic) {
								music.Play ();
						} else {
								music.Pause ();
						}
				}
		}

		/// <summary>
		/// Gets the faction color from it ID
		/// </summary>
		/// <returns>The faction color from I.</returns>
		/// <param name="factionID">Faction I.</param>
		Color GetFactionColorFromID (int factionID)
		{
				Color factionColor = MyColor.black;
		
				switch (factionID) {
				case 500000:	// No faction
						factionColor = MyColor.dimGray;
						break;
			
				case 500001:	// Caldari
						factionColor = MyColor.deepSkyBlue;
						break;
			
				case 500002:	// Minmatar
						factionColor = MyColor.chocolate;
						break;
			
				case 500003:	// Amarr
						factionColor = MyColor.gold;
						break;
			
				case 500007:	// Anmatar Mandate
						factionColor = MyColor.orangeRed;
						break;
			
				case 500008:	// Khandid Kingdom
						factionColor = MyColor.orange;
						break;
			
				case 500004:	// Gallente
						factionColor = MyColor.lime;
						break;
			
				case 500005:	// Jove
						factionColor = MyColor.silver;
						break;
			
				case 500006:	// CONCORD
						factionColor = MyColor.lightSkyBlue;
						break;
			
				case 500009:	// The Syndicate
						factionColor = MyColor.dimGray;
						break;
			
				case 500010:	// Guristas Pirates
						factionColor = MyColor.goldenRod;
						break;
			
				case 500011:	// Angel Cartel
						factionColor = MyColor.silver;
						break;
			
				case 500012:	// Blood Raider Covenant
						factionColor = MyColor.fireBrick;
						break;
			
				case 500013:	// The Interbus
						factionColor = MyColor.white;
						break;
			
				case 500014:	// ORE
						factionColor = MyColor.yellow;
						break;
			
				case 500015:	// Thukker Triber
						factionColor = MyColor.peru;
						break;
			
				case 500016:	// Sisters of Eve
						factionColor = MyColor.dimGray;
						break;
			
				case 500017:	// SoCT
						factionColor = MyColor.lightGray;
						break;
			
				case 500018:	// Mordu's Legion
						factionColor = MyColor.white;
						break;
			
				case 500019:	// Sansha's Nation
						factionColor = MyColor.darkOliveGreen;
						break;
			
				case 500020:	// Serpentis
						factionColor = MyColor.oliveDrab;
						break;			
				}
		
				return factionColor;
		}

		/// <summary>
		/// Returns a random color with a brightness over 30%
		/// </summary>
		/// <returns>The random region color.</returns>
		Color GenerateRandomRegionColor ()
		{
				Color regionColor = Color.gray;	// Fallback color when no other is found
				float brightness = 0;
				int safetyCounter = 0;			// Safety counter to prevend an infinite loop
				
				while ((brightness < 0.3F) && (safetyCounter < 10000)) {
						regionColor = new Color (UnityEngine.Random.Range (0F, 1F), UnityEngine.Random.Range (0F, 1F), UnityEngine.Random.Range (0F, 1F));
						brightness = (2F * regionColor.r + regionColor.b + 3F * regionColor.g) / 6F;
						safetyCounter++;
				}
		
				return (regionColor);
		}


		/// <summary>
		/// Converts the ID from a CSV string.
		/// </summary>
		/// <returns>The identifier from string.</returns>
		/// <param name="input">Input.</param>
		private int ConvertIDFromString (string input)
		{
				int numVal = -1;
		
				if (input != null && (input.Length > 0)) {	// only execute for non-empty strings
						try {
								numVal = Convert.ToInt32 (input);
						} catch (SystemException) {
								// if any exception from conversion (i.e. invalid data for int) than just put 0 as value
								numVal = 0;
//				Debug.LogWarning ("Conversion not possible for ConvertIDFromString: " + input + " - Details: " + e);
						}
				}

				return numVal;
		}

		Color ConvertHexColor (string input)
		{
				if (input.Length < 6) {
						throw new System.IndexOutOfRangeException ("ConvertHexColor(): invalid input string");
				}
		
				byte r = Convert.ToByte (input.Substring (0, 2), 16);
				byte g = Convert.ToByte (input.Substring (2, 2), 16);
				byte b = Convert.ToByte (input.Substring (4, 2), 16);
		
				return new Color32 (r, g, b, 255);		
		}

		private void  CreateSolarColorMaterials (Material sysBasisMat)
		{
				
				// Mapping solar colar according to Havard spectral classification using the "Actual apparant color" according to wikipedia.com
				
				Material mat;
				
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("9bb0ff");
				solarColorMat.Add ('O', mat);
				
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("aabfff");
				solarColorMat.Add ('B', mat);
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("cad8ff");
				solarColorMat.Add ('A', mat);
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("fbf8ff");
				solarColorMat.Add ('F', mat);
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("fff4e8");
				solarColorMat.Add ('G', mat);
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("ffddb4");
				solarColorMat.Add ('K', mat);
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("ffbd6f");
				solarColorMat.Add ('M', mat);
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("f84235");
				solarColorMat.Add ('L', mat);
		
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("ba3059");
				solarColorMat.Add ('T', mat);
		
				mat = new Material (sysBasisMat);
				mat.color = ConvertHexColor ("000000");
				solarColorMat.Add ('Y', mat);
		
		
		}

		/// <summary>
		/// Calculates the quad geometry for drawing a pseudo line in 3D space
		/// </summary>
		/// <returns>The quad.</returns>
		/// <param name="s">S.</param>
		/// <param name="e">E.</param>
		/// <param name="w">The width.</param>
		private Vector3[] MakeQuad (Vector3 s, Vector3 e, float w)
		{
				w = w / 2;
				Vector3[] q = new Vector3[4];
				Vector3 n = Vector3.Cross (s, e);
				Vector3 l = Vector3.Cross (n, e - s);
	
				// Using perpendicular vector trick to normalize l in order to improve performance
				// l.Normalize();
				float lLength = l.magnitude;
				l /= lLength;

/*	Only needed if the root Game Object of this script is not on (0 / 0 / 0) position
		q[0] = transform.InverseTransformPoint(s + l * w);
		q[1] = transform.InverseTransformPoint(s - l * w);
		q[2] = transform.InverseTransformPoint(e + l * w);
		q[3] = transform.InverseTransformPoint(e - l * w);
*/	
				q [0] = (s + l * w);
				q [1] = (s - l * w);
				q [2] = (e + l * w);
				q [3] = (e - l * w);
		
				return q;
		}


		// Calculates another quad which is 90 deg rotated to the other quad. 
		private Vector3[] MakeQuad2 (Vector3 s, Vector3 e, float w)
		{
				w = w / 2;
				Vector3[] q = new Vector3[4];
				Vector3 n = Vector3.Cross (s, e);
		
				Vector3 l = Vector3.Cross (n, e - s);
				l = Vector3.RotateTowards (l, n, -90f * Mathf.Deg2Rad, 0f);
		
				// Using perpendicular vector trick to normalize l in order to improve performance
				// l.Normalize();
				float lLength = l.magnitude;
				l /= lLength;

		
								
				/*	Only needed if the root Game Object of this script is not on (0 / 0 / 0) position
		q[0] = transform.InverseTransformPoint(s + l * w);
		q[1] = transform.InverseTransformPoint(s - l * w);
		q[2] = transform.InverseTransformPoint(e + l * w);
		q[3] = transform.InverseTransformPoint(e - l * w);
*/	
				q [0] = (s + l * w);
				q [1] = (s - l * w);
				q [2] = (e + l * w);
				q [3] = (e - l * w);
		
				return q;
		}
	
		/// <summary>
		/// Adds the calculated line to the mesh
		/// </summary>
		/// <param name="m">M.</param>
		/// <param name="quad">Quad.</param>
		/// <param name="tmp">If set to <c>true</c> tmp.</param>
		private void AddLine (Mesh m, Vector3[] quad, bool tmp)
		{
				int vl = m.vertices.Length;
	
				Vector3[] vs = m.vertices;
	
				if (!tmp || vl == 0)
						vs = ResizeVertices (vs, 4);
				else
						vl -= 4;
	
				vs [vl + 0] = quad [0];
				vs [vl + 1] = quad [1];
				vs [vl + 2] = quad [2];
				vs [vl + 3] = quad [3];
	
				int tl = m.triangles.Length;
	
				int[] ts = m.triangles;
	
				if (!tmp || tl == 0)
						ts = ResizeTraingles (ts, 6);
				else
						tl -= 6;

				ts [tl] = vl;
				ts [tl + 1] = vl + 1;
				ts [tl + 2] = vl + 2;
				ts [tl + 3] = vl + 1;
				ts [tl + 4] = vl + 3;
				ts [tl + 5] = vl + 2;
	
				m.vertices = vs;
				m.triangles = ts;

				m.RecalculateBounds ();
		}

/*
	private void AddLine2(Mesh m, Vector3 s, Vector3 e, float w, bool tmp) 
	{
		// Calculate quad
		w = w / 2;
		Vector3[] q = new Vector3[4];
		Vector3 n = Vector3.Cross(s, e);
		Vector3 l = Vector3.Cross(n, e-s);
		
		// Using perpendicular vector trick to normalize l in order to improve performance
		// l.Normalize();
		float lLength = l.magnitude;
		l /= lLength;
	
		q[0] = (s + l * w);
		q[1] = (s - l * w);
		q[2] = (e + l * w);
		q[3] = (e - l * w);
				
		// Normalize n
		lLength = n.magnitude;
		n /= lLength;
				
		// Building mesh
		int vl = m.vertices.Length;
		
		Vector3[] vs = m.vertices;
		
		if(!tmp || vl == 0) vs = ResizeVertices(vs, 4);
		else vl -= 4;
		
		vs[vl+0] = q[0];
		vs[vl+1] = q[1];
		vs[vl+2] = q[2];
		vs[vl+3] = q[3];
		
		int tl = m.triangles.Length;
		
		int[] ts = m.triangles;
		
		if(!tmp || tl == 0) ts = ResizeTraingles(ts, 6);
		else tl -= 6;
		
		ts[tl] = vl;
		ts[tl+1] = vl+1;
		ts[tl+2] = vl+2;
		ts[tl+3] = vl+1;
		ts[tl+4] = vl+3;
		ts[tl+5] = vl+2;
		
	
		m.vertices = vs;
		m.triangles = ts;
		
		m.RecalculateBounds();
		
	}
*/	

		private Vector3[] ResizeVertices (Vector3[] ovs, int ns)
		{
				Vector3[] nvs = new Vector3[ovs.Length + ns];
				for (int i = 0; i < ovs.Length; i++)
						nvs [i] = ovs [i];
				return nvs;
		}
	
		int[] ResizeTraingles (int[] ovs, int ns)
		{
				int[] nvs = new int[ovs.Length + ns];
				for (int i = 0; i < ovs.Length; i++)
						nvs [i] = ovs [i];
				return nvs;
		}

		/// <summary>
		/// Updates the mesh with texture coordinates
		/// </summary>
		/// <param name="m">M.</param>
		private void UpdateMeshUvs (Mesh m)
		{
				Vector3[] vs = m.vertices;

				Vector2[] uvs = new Vector2[vs.Length];

				int i = 0;
				while (i < uvs.Length) {
						uvs [i] = new Vector2 (vs [i].x, vs [i].z);
						i++;
				}
				m.uv = uvs;
		}


}

