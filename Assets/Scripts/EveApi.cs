/*
 * EveApi.cs
 *
 * Copyright (C) 2015 Erik Kalkoken
 *
 * Wrapper class for accessing the XML API of Eve Online
 *
 * HISTORY
 * 21-FEB-2016 v0.2 Improvements to getSystemStats method
 * 20-DEC-2015 v0.1 Initial version
 *
**/

using System.Xml;
using System.Net;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Wrapper class for accessing the XML API of Eve Online
/// </summary>
public static class EveApi
{
    const string eveXmlApiUrl = "https://api.eveonline.com";
	enum SystemStatsType { kills, jumps };

	/// <summary>
	/// Disables validation of certificates for HTTPS since mono does not use the client CA
	/// </summary>
    public static void initHttps()
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
               delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                       System.Security.Cryptography.X509Certificates.X509Chain chain,
                                       System.Net.Security.SslPolicyErrors sslPolicyErrors)
               {
                   return true; // **** Always accept
           };
    }

    /*
	public static string getSystems (int regionId)
    {
        string url = "http://kalkoken.altervista.org/scripts/killsearch/getEveSystems.php?regionid=" + regionId;
        Debug.Log("Loading page from url " + url);
                
        string result = new WebClient().DownloadString(url);

        return result;
    }
    */

	/// <summary>
	/// returns kills per system from last hour in Eve Online as <systemId, kills>
	/// kills = shipsKills + podKills
	/// </summary>
	public static Dictionary<int, int> getKills () { return getSystemStats (SystemStatsType.kills);	}

	/// <summary>
	/// returns jumps per system from last hour in Eve Online as <systemId, jumps>
	/// </summary>
	public static Dictionary<int, int> getJumps () { return getSystemStats (SystemStatsType.jumps);	}

	/// <summary>
	/// returns either jumps or kills from last hour in Eve Online as <systemId, detail>
	/// kills = shipsKills + podKills
	/// returns null on error
	/// </summary>
	private static Dictionary<int, int> getSystemStats (SystemStatsType type)
    {
        Dictionary<int, int> systemDetails = new Dictionary<int, int>();
		string url = "";
		bool success = true;

		if (type == SystemStatsType.kills)
		{
        	url = eveXmlApiUrl + "/map/kills.xml.aspx";
			// url = "C:\\Users\\bji74\\Desktop\\kills.xml"; // for development
		}
		else
		{
			url = eveXmlApiUrl + "/map/jumps.xml.aspx";
			// url = "C:\\Users\\bji74\\Desktop\\jumps.xml"; // for development
		}				

		// Debug.Log("Loading page from url " + url);

        // Read definition of Price Index from file
        XmlDocument doc = new XmlDocument();

        try
        {
            doc.Load(url);
        }
        catch (Exception ex)
        {
			ErrorOccured("xml file could not be loaded", ex);
			success = false;
        }

        if (success)
	    {
	        // get list of items
	        try
	        {
	            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/eveapi/result/rowset/row");
	            foreach (XmlNode node in nodes)
	            {
	                XmlAttributeCollection row = node.Attributes;
	                                
					int solarSystemID = 0, detail1 = 0, detail2 = 0;;
	                foreach (XmlAttribute column in row)
	                {
	                    if (column.Name == "solarSystemID") solarSystemID = Convert.ToInt32(column.InnerText);
	                    if (type == SystemStatsType.kills)
	                   	{					 
							if (column.Name == "shipKills") detail1 = Convert.ToInt32(column.InnerText);
							if (column.Name == "podKills") detail2 = Convert.ToInt32(column.InnerText);
						}
						else
						{
							if (column.Name == "shipJumps") detail1 = Convert.ToInt32(column.InnerText);
						}
	                }
					if (solarSystemID != 0) systemDetails.Add(solarSystemID, detail1 + detail2);
	            }
	        }
	        catch (Exception ex)
	        {
				ErrorOccured("Could not extract items from xml file", ex);
				success = false;
	        }
		}

		if (success) return systemDetails;
		else return null;
    }

	/// <summary>
	/// returns the top systems in desending order as dictionary
	/// systemDetails: list of systemDetails to process
	/// max: max number of systems the list should contain
	/// </summary>
	public static Dictionary<int, int> getTop (Dictionary<int, int> systemDetails, int max)
	{
		Dictionary<int, int> result = new  Dictionary<int, int>();
		int counter = 0;

				var query = systemDetails.OrderByDescending (item => item.Value);

		foreach (KeyValuePair<int,int> item in query)
		{ 
			result.Add (item.Key, item.Value);
			if (++counter > max) break;
		}

		return result;
	}

	/// <summary>
	/// Sends an fatal error message to Log and quits the application
	/// </summary>
	private static void FatalErrorOccured (string text, Exception ex)
    {
        Debug.LogError("Fatal error occured: " + text + " Exception: " + ex);
		Application.Quit();
    }

	/// <summary>
	/// Sends an error message to Log
	/// </summary>
	private static void ErrorOccured (string text, Exception ex)
    {
        Debug.LogError("Error occured: " + text + " Exception: " + ex);
    }
}
