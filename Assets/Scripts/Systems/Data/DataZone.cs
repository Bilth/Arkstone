using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PACK
{

}

public class DataZone : MonoBehaviour {

    public List<PACK> packs;

    private int _id;

    public int ID
    {
        get; set;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual int A { get; set; }
}
