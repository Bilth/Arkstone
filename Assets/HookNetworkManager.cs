using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HookNetworkManager : MonoBehaviour {

    NetworkManager tNetMan;

	// Use this for initialization
	void Start () {
        tNetMan = NetworkManager.singleton;
        tNetMan.StartHost();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
