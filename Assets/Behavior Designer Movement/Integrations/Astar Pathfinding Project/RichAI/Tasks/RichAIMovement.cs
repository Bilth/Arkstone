using UnityEngine;
using Pathfinding;
using Pathfinding.RVO;
using System;

namespace BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject.RichAI
{
    // Abstract class for any task that uses an RichAIAgent
    public abstract class RichAIMovementAgent : Movement
    {
        [Tooltip("The max speed of the agent")]
        public SharedFloat maxSpeed = 5;
        [Tooltip("Rotation speed of the agent")]
        public SharedFloat rotationSpeed = 360;

        // A cache of the RichAI
        private RichAIAgent richAIAgent;
        // A cache of the RVOController (if used)
        private RVOController rvoController;

        public override void OnAwake()
        {
            // cache for quick lookup
            richAIAgent = gameObject.GetComponent<RichAIAgent>();
            rvoController = gameObject.GetComponent<RVOController>();

            if (richAIAgent.target == null) {
                var target = new GameObject();
                target.name = Owner.name + " Target";
                richAIAgent.target = target.transform;
                richAIAgent.repeatedlySearchPaths = false;
            }
        }

        public override void OnStart()
        {
            richAIAgent.maxSpeed = maxSpeed.Value;
            richAIAgent.rotationSpeed = rotationSpeed.Value;
        }

        protected override bool SetDestination(Vector3 target)
        {
            if (richAIAgent.target.position != target) {
                richAIAgent.target.position = target;
                richAIAgent.UpdatePath();
            }
            return true;
        }

        protected override bool HasArrived()
        {
            return richAIAgent.PathCalculated() && richAIAgent.TargetReached;
        }

        protected override Vector3 Velocity()
        {
            return richAIAgent.Velocity;
        }
        protected override void UpdateRotation(bool update)
        {
            // Intentionally left blank
        }

        protected override bool HasPath()
        {
            return richAIAgent.PathCalculated();
        }

        protected override void Stop()
        {
            if (rvoController != null) {
                rvoController.Move(Vector3.zero);
            }
        }

        public override void OnEnd()
        {
            Stop();
        }

        public override void OnReset()
        {
            maxSpeed.Value = 5;
            rotationSpeed.Value = 360;
        }
    }
}