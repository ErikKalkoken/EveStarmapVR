/*
 * zKillApi.cs
 *
 * Copyright (C) 2015 Erik Kalkoken
 *
 * Wrapper class for accessing the API of zKillboard
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
/// Wrapper class for accessing the API of zKillboard
/// </summary>
public static class zkbApi
{
    const string apiUrl = "https://zkillboard.com/api";

	public class ZkillVictim
    {
		public int shipTypeID  { get; set; }
		public int characterID { get; set; }
		public string characterName { get; set; }
		public int corporationID { get; set; }
		public string corporationName { get; set; }
		public int allianceID { get; set; }
		public string allianceName { get; set; }
		public int factionID { get; set; }
		public string factionName { get; set; }
		public int damageTaken { get; set; }
    }

	public class ZkillPosition
    {
		public double x { get; set; }
		public double y { get; set; }
		public double z { get; set; }
    }

	public class ZkillZkb
    {
		public int locationID { get; set; }
		public string hash { get; set; }
		public double totalValue { get; set; }
		public int points { get; set; }
		public int involed { get; set; }
    }

    public class Killmail
    {
    	public int killID { get; set; }
		public int solarSystemID { get; set; }
		public DateTime killTime { get; set; }
		public int moonID { get; set; }
		public ZkillVictim victim { get; set; }
		// public ZkillPosition position { get; set; }
		// public ZkillZkb zkb { get; set; }

		public Killmail()
		{
			victim = new ZkillVictim();
		}
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

	/// <summary>
	/// Retrieves all killmails of the last 24hrs and returns them as List
	/// returns null on error
	/// </summary>
	public static List<Killmail> getKillsToday ()
    {
    	const int MAXPAGES = 1;
		bool success = true;
		List<Killmail> killmails = new List<Killmail>();

		DateTime startDT = DateTime.Now.AddHours(-24);
		string baseUrl = "https://zkillboard.com/api/kills/xml/startTime/" + startDT.ToString("yyyyMMddHH0000") + "/page/";

		// string url = "C:\\Users\\bji74\\Desktop\\killmails2.xml"; // for development								


		XmlDocument doc = new XmlDocument();


		for (int page = 1; page <= MAXPAGES; page++)
		{
			string url = baseUrl + page + "/";
			int killmailcount = 0;

	        try
	        {
				Log("Loading page from url: " + url);
				string xml = xmlApiWrap.apiRequest (url);
				doc.LoadXml(xml);
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
		            	
		            	if (node.Name == "row")
		                {
							Killmail killmail = new Killmail();

							XmlAttributeCollection row = node.Attributes;
							foreach (XmlAttribute column in row)
			                {
								if (column.Name == "killID") killmail.killID = Convert.ToInt32(column.InnerText);
								if (column.Name == "solarSystemID") killmail.solarSystemID = Convert.ToInt32(column.InnerText);
								if (column.Name == "killTime") killmail.killTime = DateTime.ParseExact(column.InnerText, "yyyy-MM-dd HH:mm:ss", null);
								if (column.Name == "moonID") killmail.moonID = Convert.ToInt32(column.InnerText);
			                }
							
							XmlNodeList subNodes = node.ChildNodes;
							foreach (XmlNode subNode in subNodes)
		            		{
		            			if (subNode.Name == "victim")
				                {
									XmlAttributeCollection subRow = subNode.Attributes;
									foreach (XmlAttribute column in subRow)
					                {
										if (column.Name == "shipTypeID") killmail.victim.shipTypeID = Convert.ToInt32(column.InnerText);
										if (column.Name == "characterID") killmail.victim.characterID = Convert.ToInt32(column.InnerText);
										if (column.Name == "characterName") killmail.victim.characterName = column.InnerText;
										if (column.Name == "corporationID") killmail.victim.corporationID = Convert.ToInt32(column.InnerText);
										if (column.Name == "corporationName") killmail.victim.corporationName = column.InnerText;
										if (column.Name == "allianceID") killmail.victim.allianceID = Convert.ToInt32(column.InnerText);
										if (column.Name == "allianceName") killmail.victim.allianceName = column.InnerText;
										if (column.Name == "factionID") killmail.victim.factionID = Convert.ToInt32(column.InnerText);
										if (column.Name == "factionName") killmail.victim.factionName = column.InnerText;
										if (column.Name == "damageTaken") killmail.victim.damageTaken = Convert.ToInt32(column.InnerText);

					                }
								}
							}
							killmails.Add(killmail);
							killmailcount++;
						}
		            }
		        }
		        catch (Exception ex)
		        {
					ErrorOccured("Could not extract items from xml file", ex);
					success = false;
		        }
			}
			else
			{
				// exit for-loop if page load was unsuccessful
				break;
			}

			// exit for-loop if less than 200 killmails on current page
			if (killmailcount < 200) break;
		}

		if (success)
		{
			Log ("Received a total of " + killmails.Count + " killmails");
			return killmails;
		}
		else
		{
			Log ("Failed to receive killmails");
			return null;
		}
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

	public static void Log (string text)
    {
        Debug.Log (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + text);
    }
}
