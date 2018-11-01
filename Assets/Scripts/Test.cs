using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {

        EveApi.initHttps();

        Dictionary<int, int> systemKills = EveApi.getKills();

        foreach (var item in systemKills)
        {
            Debug.Log(item.Key + " - " + item.Value);
        }

    }
	
}
