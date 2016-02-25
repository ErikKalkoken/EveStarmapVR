/*
 * xmlApiWrap.cs
 *
 * Copyright (C) 2015 Erik Kalkoken
 *
 * Wrapper class for caching XML API calls to Eve and ZKB API
 *
 * HISTORY
 * 25-FEB-2016 v1.0 Initial version based on simliar methods in EveApi.php
 *
**/

using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Net;
using System;

public class xmlApiWrap
{

	/// <summary>
	/// Retrieves XML from Eve or zkb api by url with caching
	/// Uses cachedUntil tag in XML to determine if cache file is expired
	/// returns xml content as string or null on error
	/// </summary>
	static public string apiRequest (string apiurl)
	{
		string rawdata = "";

		string filename = generateCacheFilename (apiurl);					
		rawdata = loadFromCache(filename, false);
								
		if (rawdata == null)
		{
			Debug.Log ("Could not read from cache. Trying to fetch data from server");
			rawdata = loadFromServer (apiurl, filename);
			
			// try to use old cache data if data cound not be loaded from server
			if (rawdata == null)
			{
				rawdata = loadFromCache(filename, true);
			}
		}
		else
		{
			Debug.Log ("Data read from cache");
		}
		
		if (rawdata != null)
		{
			Debug.Log ("API request successful");
		}
		else
		{
			Debug.Log ("API request failed. Could not get data from either cache file or API");
		}

		return rawdata;
	}	

	/// <summary>
	/// Loads XML from cache file
	/// Uses cachedUntil tag in XML to determine if cache file is expired
	/// returns xml content as string or null on error
	/// ignoreTimestamp: when set to true will ignore cachedUntil tag and always use cache file (if it exists)
	/// </summary>
	static private string loadFromCache(string filename, bool ignoreTimestamp=false)
	{
		bool refresh_required = false;
		string rawdata = "";
		
		// Logger::debug ("Trying to read from cache:" . $filename);
		
		if ( File.Exists (filename) )
		{
						
			// read file exists if it exists
			bool success = true;
			XmlDocument doc = new XmlDocument();
			try
	        {
				rawdata = File.ReadAllText (filename);
				doc.LoadXml (rawdata);
	        }
	        catch (Exception ex)
	        {
				ErrorOccured("xml file could not be loaded", ex);
				success = false;
	        }

			if (success)
		    {
			   // get list of items
		       DateTime cachedUntil = DateTime.Now;
		       try
		       {
		       		XmlNode node2 = doc.DocumentElement.SelectSingleNode ("/eveapi/cachedUntil");
					cachedUntil = DateTime.ParseExact(node2.InnerText, "yyyy-MM-dd HH:mm:ss", null);
				}
		        catch (Exception ex)
		        {
					ErrorOccured("Could not extract items from xml file", ex);
					success = false;
		        }

		        if (success)
		        {
					bool expired = ( cachedUntil < DateTime.UtcNow )  ? true : false;
					refresh_required = ( expired && !ignoreTimestamp ) ? true : false;
										
					// Logger::debug ("Timestamp of cached data '" . date (Utility::TIMESTAMP_STANDARD, $currentTime) . "' - expires at '" . date (Utility::TIMESTAMP_STANDARD, $cachedUntil) . "'");
					// Logger::debug ("Refresh required: " . var_export ($refresh_required, true)  . "'");
				}
				else
				{
					// xmls elements not found in file
					refresh_required = true;
				}
			}
			else
			{
				// file could not be loaded
				refresh_required = true;
			
			}
		}
		else
		{
			// file does not exist, needs to be refreshed
			refresh_required = true;
		}

		if (!refresh_required)	return rawdata;
		else return null;
	}

	/// <summary>
	/// write fresh request result to cache file	
	/// </summary>
	static private void writeToCache (string filename, string xml)
	{
		bool success = true;
		//Utility::makeDir ("temp");	// create folder in case it does not exist

		try
		{
			File.WriteAllText (filename, xml);
		}
		catch (Exception ex)
		{
			ErrorOccured("Could not extract items from xml file", ex);
			success = false;
		}
		
		if (!success)
		{
			// Logger::warn ("Could not write to cache file '" . $filename . "'");
		}
		else
		{
			// Logger::info ("Updated cache file");
		}
	}

	/// <summary>
	/// fetches XML data from server with apiurl, saves results to cache file
	/// returns xmls data as string
	/// </summary>
	static private string loadFromServer (string apiurl, string filename)
	{
		bool success = true;
		string xml = "";

		// get fresh data from API, returns XML
		try
		{
			xml = new WebClient().DownloadString(apiurl);
		}
		catch (Exception ex)
		{
			ErrorOccured("Could not retrieve page from server at url " + apiurl, ex);
			success = false;
		}

		if (success)
		{
			try
			{
				writeToCache (filename, xml);
			}
			catch (Exception ex)
			{
				ErrorOccured("Could not store xml for url " + apiurl, ex);
			}
			return xml;
		}
		else return null;
	}

	/// <summary>
	/// returns filename of current page. filename is generated has has based on url
	/// path of filename is the local temp folder
	/// </summary>
	static private string generateCacheFilename (string url)
	{
		string path = System.IO.Path.GetTempPath() + "evestarmap";
		System.IO.Directory.CreateDirectory(path);

		string filename = path + "\\apicache_" + url.GetHashCode() + ".tmp";
		Debug.Log ("Filename used is " + filename);
		return filename;
	}

	private static void ErrorOccured (string text, Exception ex)
    {
        Debug.LogError("Error occured: " + text + " Exception: " + ex);
    }

	public static void Log (string text)
    {
        Debug.Log (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + text);
    }
}
