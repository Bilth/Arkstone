using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{

    public Vector3 velocity = Vector3.zero;

    void Update()
    {
        transform.position = transform.position + velocity * Time.deltaTime;
    }
}

