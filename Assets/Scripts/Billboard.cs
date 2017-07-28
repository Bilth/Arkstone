using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        Camera tCam = Camera.main;
        if (tCam != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
