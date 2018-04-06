using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{

    public Vector3 startingVelocity = Vector3.zero;

    private Rigidbody _body;

    void Start()
    {
        _body = GetComponent<Rigidbody>();

        Vector3 tVelocity = _body.velocity;
        _body.velocity = startingVelocity;
    }

    void Update()
    {
       // transform.Rotate(new Vector3(xPerSecond * Time.deltaTime, yPerSecond * Time.deltaTime, zPerSecond * Time.deltaTime));
    }
}

