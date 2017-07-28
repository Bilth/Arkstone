using UnityEngine;
using System.Collections;

public class RotateContinously : MonoBehaviour {
	
	public float xPerSecond = 0;
    public float yPerSecond = 0;
    public float zPerSecond = 0;
    
    void Update () {
		transform.Rotate (new Vector3(xPerSecond * Time.deltaTime, yPerSecond * Time.deltaTime, zPerSecond * Time.deltaTime));
	}
}
