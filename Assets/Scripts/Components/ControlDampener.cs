using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDampener : MonoBehaviour
{
    private const float REGAIN_PERCENT = 0.33f;

    private float _remaining = 0.0f; // Percent

    private float _durationMax = 0.0f;
    private float _duration = 0.0f;
    public float Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }

    private float _multiplier = 1.0f;
    public float Multiplier
    {
        get {  return _multiplier; }
        set { _multiplier = value; }
    }
	
	void Update () {
        if(_duration > 0)
        {
            _duration -= Time.deltaTime;
            if(_duration <= 0) { _duration = 0; }

            _remaining = _duration / _durationMax;
            if(_remaining < REGAIN_PERCENT)
            {
                _multiplier = 1.0f - (_remaining / REGAIN_PERCENT);
            } else
            {
                _multiplier = 0.0f;
            }
        }
    }

    public void AddDampener(float pDuration)
    {
        if(pDuration >= _duration) {
            _durationMax = _duration = pDuration;
        }
    }

    public void Clear()
    {
        _duration = 0;
        _multiplier = 1.0f;
    }
}
