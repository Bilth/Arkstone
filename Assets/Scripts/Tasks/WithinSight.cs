﻿using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Arkstone")]
public class WithinSight : Conditional
{
    // How wide of an angle the object can see
    public float fieldOfViewAngle;
    // The tag of the targets
    public string targetTag;
    // Set the target variable when a target has been found so the subsequent tasks know which object is the target
    public SharedGameObject target;

    // A cache of all of the possible targets
    private GameObject[] possibleTargets;

    public override void OnAwake()
    {
        // Cache all of the transforms that have a tag of targetTag
        possibleTargets = GameObject.FindGameObjectsWithTag(targetTag);
    }

    public override TaskStatus OnUpdate()
    {
        // Return success if a target is within sight
        for (int i = 0; i < possibleTargets.Length; ++i)
        {
            if (withinSight(possibleTargets[i], fieldOfViewAngle))
            {
                // Set the target so other tasks will know which transform is within sight
                target.SetValue(possibleTargets[i]);
                return TaskStatus.Success;
            }
        }
        return TaskStatus.Failure;
    }

    // Returns true if targetTransform is within sight of current transform
    public bool withinSight(GameObject targetObject, float fieldOfViewAngle)
    {
        Vector3 direction = targetObject.transform.position - transform.position;
        // An object is within sight if the angle is less than field of view
        return Vector3.Angle(direction, transform.forward) < fieldOfViewAngle;
    }
}