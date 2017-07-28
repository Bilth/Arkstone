using UnityEngine;
using System.Collections;

public class Starmap : MonoBehaviour
{
    private Vector3 _rotation;
    private Vector3 _rotationRate;
    
    void Start()
    {
        _rotation = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        _rotationRate = new Vector3(.5f, 0f, 0f);
        //_rotationRate = Random.Range(0, ROTATION_RATE);
    }
    
    void Update()
    {
        _rotation.x += _rotationRate.x * Time.deltaTime;
        _rotation.y += _rotationRate.y * Time.deltaTime;
        _rotation.z += _rotationRate.z * Time.deltaTime;

        //Quaternion.Euler
        //_cam.transform.localEulerAngles = new Vector3(_cameraRotationXCurrent, 0f, 0f);
        transform.localEulerAngles = _rotation;
    }
}
