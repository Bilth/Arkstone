using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PID
{
    public float P, I, D;

    float integral;
    float lastError;

    public float Update(float setPoint, float position)
    {
        float error = setPoint - position;
        integral += error * Time.deltaTime;

        float derivative = (error - lastError) / Time.deltaTime;
        lastError = error;

        return error * P + integral * I + derivative * D;
    }
}