using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

    private float ROTATION_RATE = 2f;

    float _rotation = 0f;
    float _rotationRate = 0f;

	// Use this for initialization
	void Start () {
        _rotation = transform.localEulerAngles.y;
        _rotationRate = Random.Range(0, ROTATION_RATE);
    }
	
	// Update is called once per frame
	void Update () {
        _rotation += _rotationRate * Time.deltaTime;
     
        //Quaternion.Euler
        //_cam.transform.localEulerAngles = new Vector3(_cameraRotationXCurrent, 0f, 0f);
        transform.localEulerAngles = new Vector3(0, _rotation, 0);
	}
}
