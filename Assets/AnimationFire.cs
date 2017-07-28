using UnityEngine;
using System.Collections;

public class AnimationFire : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Animation>().Play("Ligth Fire Animation");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
