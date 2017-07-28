using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void PlaySound(string s)
    {
        Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
    }
}
