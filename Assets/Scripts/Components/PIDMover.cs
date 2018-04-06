using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDMover : MonoBehaviour
{
    public float Distance = 3f;
    public PID Controller;

    public Transform Sensor;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
       // var force = Mathf.Max(0, Controller.Update(Distance, hit.distance));
       // rb.AddForce(transform.up * force);
    }
}