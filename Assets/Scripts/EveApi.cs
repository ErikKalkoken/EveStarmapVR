/*
 * EveApi.cs
 *
 * Copyright (C) 2015 Erik Kalkoken
 *
 * Wrapper class for accessing the XML API of Eve Online
 *
 * HISTORY
 * 01-NOV-2018 v0.3 Change: Migration to ESI API
 * 21-FEB-2016 v0.2 Improvements to getSystemStats method
 * 20-DEC-2015 v0.1 Initial version
 *
**/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;





/// <summary>
/// Wrapper class for accessing the XML API of Eve Online
/// </summary>
public static class EveApi
{
    const string eveEsiUrlSystemKills = "https://esi.evetech.net/v2/universe/system_kills/";
    const string eveEsiUrlSystemJumps = "https://esi.evetech.net/v1/universe/system_jumps/";
    enum SystemStatsType { kills, jumps };

    [Serializable]
    public class EveSystemKill
    {
        public int npc_kills;
        public int pod_kills;
        public int ship_kills;
        public int system_id;
    }

    [Serializable]
    public class EveSystemJump
    {
        public int ship_jumps;        
        public int system_id;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    public static string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

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
	public static Dictionary<int, int> getKills () 
    {
        Dictionary<int, int> systemDetails = new Dictionary<int, int>();

        try
        {
            WebClient client = new WebClient();
            string getString = client.DownloadString(eveEsiUrlSystemKills);

            EveSystemKill[] systems = JsonHelper.FromJson<EveSystemKill>(fixJson(getString));

            foreach (EveSystemKill system in systems)
            {
                int totalKills = system.pod_kills + system.ship_kills;
                if (totalKills > 0)
                {
                    systemDetails.Add(system.system_id, totalKills);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Warnung: failed to fetch system kills from ESI - Exception: " + ex);
        }

        return systemDetails;
    }

    /// <summary>
    /// returns jumps per system from last hour in Eve Online as <systemId, jumps>
    /// </summary>
    public static Dictionary<int, int> getJumps ()
    {
        Dictionary<int, int> systemDetails = new Dictionary<int, int>();

        try
        {
            WebClient client = new WebClient();
            string getString = client.DownloadString(eveEsiUrlSystemJumps);

            EveSystemJump[] systems = JsonHelper.FromJson<EveSystemJump>(fixJson(getString));

            foreach (EveSystemJump system in systems)
            {                
                systemDetails.Add(system.system_id, system.ship_jumps);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Warnung: failed to fetch system jumps from ESI - Exception: " + ex);
        }

        return systemDetails;
    }

    /*
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

        url = (type == SystemStatsType.kills)
            ? eveEsiUrlSystemKills
            : eveEsiUrlSystemJumps;

		// Debug.Log("Loading page from url " + url);

        // Read definition of Price Index from file
        XmlDocument doc = new XmlDocument();

        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        catch (Exception ex)
        {
			ErrorOccured("xml file could not be loaded", ex);
			success = false;
        }

        if (success)
	    {
	        // get list of items
	        
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

		if (success) return systemDetails;
		else return null;
    }
    */

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
