/*
 * CustomMenu.cs
 *
 * Copyright (C) 2015 Erik Kalkoken
 *
 * Helper class for storing and managing the data of the a menu and displaying it on the GUI
 *
 * HISTORY
 * 21-FEB-2016 v0.3 Adjusted font size to accomodate additional menu entries
 * 17-FEB-2016 v0.2 Adjustment due to migration to Unity 5
 * 13-DEC-2015 v0.1 Initial version
 *
**/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// CustomMenu - Helper class for storing and managing the data of the a menu and displaying it on the GUI
/// </summary>

public class CustomMenu {

	// custom data types
	struct MenuItem
	{
		public string label;
		public string[] options;
		public int chosenOptionIdx;
		public int defaultOptionIdx;
	}

	// member variables

	private List<MenuItem> items;
//	private List<int> defaultOption;
	private int currentOptionIdx;
	private int currentItemIdx;

	// member methods

	// Initialize class with items and otions
	public CustomMenu ()
	{
		currentOptionIdx = 0;
		currentItemIdx = 0;

		items = new List<MenuItem>();
	}

	public void AddItem (string iLabel, string [] iOptions)
	{
		AddItem (iLabel, iOptions, 0);
	}

	public void AddItem (string iLabel, string [] iOptions, int iDefaultOptionIdx)
	{
		// Errorhandling
		if (iLabel == null)
			throw new System.NullReferenceException ("Undefined iLabel string in AddItem");

		if (iOptions == null)
			throw new System.NullReferenceException ("Undefined iOption string in AddItem");

		if ( (iDefaultOptionIdx < 0) || (iDefaultOptionIdx > iOptions.GetLength(0)) )
			throw new System.IndexOutOfRangeException ("iDefaultOptionIdx out of Range for iOptions");

		items.Add (new MenuItem () {label=iLabel, chosenOptionIdx=iDefaultOptionIdx, defaultOptionIdx=iDefaultOptionIdx, options = iOptions});
	}

	/// <summary>
	/// Returns the current options as list.
	/// </summary>
	/// <returns>A list of the current options</returns>
	public List<int> GetCurrentOptions ()
	{
		List<int> l = new List<int> ();

		foreach (MenuItem mi in items) 
		{
			l.Add (mi.chosenOptionIdx);
		}
		return l;
	}

	/// <summary>
	/// Returns the default options as list.
	/// </summary>
	/// <returns>A list of the default options</returns>
	public List<int> GetDefaultOptions ()
	{
		List<int> l = new List<int> ();
		
		foreach (MenuItem mi in items) 
		{
			l.Add (mi.defaultOptionIdx);
		}
		return l;
	}
	
	/// <summary>
	/// Resets the default options according to the currently set options
	/// </summary>
	public void ResetDefaultOptions ()
	{
		MenuItem mtemp;

		for (int i=0; i<items.Count; i++) 
		{
			mtemp = items[i];
			mtemp.defaultOptionIdx = items[i].chosenOptionIdx;
			items[i] = mtemp;
		}


	}

	public void PickItemBelow ()
	{
		
		if (currentItemIdx < items.Count-1) 
		{
			currentItemIdx++;
			currentOptionIdx = items[currentItemIdx].chosenOptionIdx;
		}
	}
	
	public void PickOptionLeft ()
	{
		
		if (currentOptionIdx > 0)
		{
			currentOptionIdx--;
		}
	}
	
	public void PickOptionRight ()
	{
		if (currentOptionIdx < items[currentItemIdx].options.GetLength(0)-1) 
		{
			currentOptionIdx++;
		}
		
	}
	
	public void ChooseOption ()
	{
		MenuItem m = items [currentItemIdx];

		m.chosenOptionIdx = currentOptionIdx;

		items [currentItemIdx] = m;
	}


	public void DrawMenu (GUISkin skin, bool noOvrMode)
	{
		GUI.skin = skin;
		
		GUIStyle highlight = new GUIStyle( skin.label);
		GUIStyle normal = new GUIStyle( skin.label);
		normal.normal.textColor = Color.white;
		highlight.normal.textColor = MyColor.orange;

				/*
		// Test to calibrate the screen size
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.Label ("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum");
		GUILayout.EndArea ();
		*/
		var borderX = 0;
		var borderY = 0;

		borderX = 0;
		borderY = 300;
		normal.fontSize = 35;
		highlight.fontSize = 35;		

		GUILayout.BeginArea (new Rect (borderX, borderY, Screen.width-2*borderX, Screen.height-2*borderY));

		int count = items.Count;
		
		for (int i=0; i<count; i++)
		{
			if (currentItemIdx == i)
			{
				if (currentOptionIdx != items[i].chosenOptionIdx)
					GUILayout.Label (items[i].label + " : <color=white>" + items[i].options[currentOptionIdx] + "</color>", highlight);
				else
					GUILayout.Label (items[i].label + " : " + items[i].options[currentOptionIdx], highlight);
			}
			else
				GUILayout.Label (items[i].label + " : " + items[i].options[items[i].chosenOptionIdx], normal);
		}

	
		GUILayout.EndArea ();

	}
	
	public int Count
	{
		get
		{
			return items.Count;
		}
	}
	
	public void PickItemAbove ()
	{
		if (currentItemIdx > 0)
		{
			currentItemIdx--;
			currentOptionIdx = items[currentItemIdx].chosenOptionIdx;
		}
	}
	
	public int CurrentOptionNum
	{
		get
		{
			return currentOptionIdx;
		}
	}
	
	public int CurrentItemNum
	{
		get
		{
			return currentItemIdx;
		}
	}
	

}

