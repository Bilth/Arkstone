using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float Distance = 3f;
    public PID Controller;

    public Transform Sensor;
    public LayerMask GroundMask;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (!Physics.Raycast(Sensor.position, -Sensor.up, out hit, Distance * 2, GroundMask))
            return;

        var force = Mathf.Max(0, Controller.Update(Distance, hit.distance));
        rb.AddForce(transform.up * force);
        Debug.Log("Force: " + force + ", Hit Distance: " + hit.distance);
    }
}