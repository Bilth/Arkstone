using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

    void OnTriggerEnter(Collider pCollider)
    {
        var tRigidbody = pCollider.GetComponent<Rigidbody>();
        if (tRigidbody != null)
        {
            tRigidbody.AddForce(new Vector3(0, 50, 0), ForceMode.VelocityChange);
        }
    }

    /*void onTriggerEnter(Collision pCollision)
    {
        Debug.Log("TRIGGER ENTER!");
        var tRigidbody = pCollision.collider.GetComponent<Rigidbody>();
        if (tRigidbody != null)
        {
            Debug.Log("For real though");
            tRigidbody.AddForce(new Vector3(0, 100, 0), ForceMode.VelocityChange);
        }
    }

    void onCollisionEnter(Collision pCOllision)
    {
        Debug.Log("COLLISION ENTER!");
    }*/
}
