using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    /*
     *  void OnTriggerEnter(Collider pCollider)
    {
        var tRigidbody = pCollider.GetComponent<Rigidbody>();
        if (tRigidbody != null)
        {
            tRigidbody.AddForce(new Vector3(0, 50, 0), ForceMode.VelocityChange);
        }
    }*/

    void OnTriggerEnter(Collider pCollider)
    {
        var tHit = pCollider.gameObject;
        var tPlayer = tHit.GetComponent<PlayerManager>();
        if (tPlayer != null)
        {
            Debug.Log("Hit " + tHit);

            if(tPlayer.isLocalPlayer)
            {
                return;
            } else
            {
                tPlayer.TakeDamage(200);
            }
            
        }

        ExplosionDamage(transform.position, 10);

        Destroy(gameObject);
    }


    /*void OnCollisionEnter(Collision pCollision)
    {

        ExplosionDamage(pCollision.contacts[0].point, 10);

        var tHit = pCollision.gameObject;
        var tPlayer = tHit.GetComponent<PlayerManager>();

        if(tPlayer != null)
        {
            if(tPlayer.isLocalPlayer)
            {
                return;
            }

            // Take Damage
            tPlayer.TakeDamage(200);
        }

        Destroy(gameObject);
    }*/

    void ExplosionDamage(Vector3 center, float radius)
    {
        center.z += 1;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        
        int i = 0;
        while (i < hitColliders.Length)
        {
            var tGameObject = hitColliders[i].gameObject;
            var tRigidbody = hitColliders[i].GetComponent<Rigidbody>();

            /*if(tPlayer != null)
            {
                Debug.Log("RIGID: " + hitColliders[i].gameObject.name);
            }*/

            if(tRigidbody != null)
            {
                hitColliders[i].GetComponent<Rigidbody>().isKinematic = false;
                hitColliders[i].GetComponent<Rigidbody>().AddExplosionForce(3000, center, 3000);
            }

            //Debug.Log("Hit Collider: " + hitColliders[i].gameObject.name + " PlayerManager Null: " + (tPlayer == null));
            if (tGameObject.GetComponent<PlayerManager>() != null)
            {
                if (tGameObject.GetComponent<PlayerManager>().isLocalPlayer)
                {
                    //hitColliders[i].GetComponent<Rigidbody>().AddForce(500, center, 200);
                }
                    // tPlayer.TakeDamage(50);
                    Debug.Log("Hit Player");
            } else
            {
                
            }

            i++;
        }
    }
}
