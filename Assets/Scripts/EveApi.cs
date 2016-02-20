using System.Xml;
using System.Net;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public static class EveApi
{
    const string eveXmlApiUrl = "https://api.eveonline.com";
	enum SystemStatsType { kills, jumps };

    public static void FatalErrorOccured (string text, Exception ex)
    {
        Debug.Log(text + " Exception: " + ex);
		Application.Quit();
    }

    // Disables validation of certificates for HTTPS since mono does not use the client CA
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

	public static Dictionary<int, int> getKills () { return getSystemStats (SystemStatsType.kills);	}
	public static Dictionary<int, int> getJumps () { return getSystemStats (SystemStatsType.jumps);	}

	private static Dictionary<int, int> getSystemStats (SystemStatsType type)
    {
        Dictionary<int, int> systemDetails = new Dictionary<int, int>();
		string url = "";

		if (type == SystemStatsType.kills)
		{
        	// url = eveXmlApiUrl + "/map/kills.xml.aspx";
			url = "C:\\Users\\bji74\\Desktop\\kills.xml";
		}
		else
		{
			// url = eveXmlApiUrl + "/map/jumps.xml.aspx";
			url = "C:\\Users\\bji74\\Desktop\\jumps.xml";
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
            FatalErrorOccured("xml file could not be loaded", ex);
        }
		
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
            FatalErrorOccured("Could not extract items from xml file", ex);
        }
		

		return systemDetails;
    }

	public static Dictionary<int, int> getTop (Dictionary<int, int> systemList, int max)
	{
		Dictionary<int, int> result = new  Dictionary<int, int>();
		int counter = 0;

		var query = systemList.OrderByDescending (item => item.Value);

		foreach (KeyValuePair<int,int> item in query)
		{ 
			result.Add (item.Key, item.Value);
			if (++counter > max) break;
		}

		return result;
	}

	
}
