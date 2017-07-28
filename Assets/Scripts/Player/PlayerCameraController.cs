using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCameraController : MonoBehaviour
{
    private float _VELOCITY_MAX = 100f;
    private float _FOV_BASE = 70f;
    private float _FOV_MOD = 5f;

    [SerializeField]
    public Camera _cam;

    // Use this for initialization
    void Start () {
        _cam.fieldOfView = _FOV_BASE;
    }
	
	// Update is called once per frame
	void Update () {
        //var tMagnitude = GetComponent<Rigidbody>().velocity.magnitude;
        //if(tMagnitude > _VELOCITY_MAX) { tMagnitude = _VELOCITY_MAX; }

        //Debug.Log("V: " + GetComponent<Rigidbody>().velocity.magnitude);

        //_cam.fieldOfView = _FOV_BASE + ((tMagnitude / _VELOCITY_MAX) * _FOV_MOD);
    }
}
