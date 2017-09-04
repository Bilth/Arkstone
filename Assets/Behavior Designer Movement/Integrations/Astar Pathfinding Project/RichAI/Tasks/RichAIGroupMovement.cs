using UnityEngine;
using Pathfinding;
using Pathfinding.RVO;

namespace BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject.RichAI
{
    // Abstract class for any task that uses a group of RichAIAgents
    public abstract class RichAIGroupMovement : GroupMovement
    {
        [Tooltip("All of the agents")]
        public SharedGameObject[] agents = null;
        [Tooltip("The max speed of the agents")]
        public SharedFloat maxSpeed = 1;
        [Tooltip("Rotation speed of the agents")]
        public SharedFloat rotationSpeed = 360;

        private RichAIAgent[] richAIAgents;
        protected Transform[] transforms;
        private RVOController[] agentRVOControllers;

        public override void OnStart()
        {
            richAIAgents = new RichAIAgent[agents.Length];
            transforms = new Transform[agents.Length];
            agentRVOControllers = new RVOController[agents.Length];

            for (int i = 0; i < agents.Length; ++i) {
                richAIAgents[i] = agents[i].Value.GetComponent<RichAIAgent>();
                agentRVOControllers[i] = agents[i].Value.GetComponent<RVOController>();
                transforms[i] = agents[i].Value.transform;

                if (richAIAgents[i].target == null) {
                    var go = new GameObject();
                    richAIAgents[i].target = go.transform;
                    richAIAgents[i].target.name = agents[i].Value.name + " Target";
                }
            }

            // Set the speed and turning speed of all of the agents
            for (int i = 0; i < agents.Length; ++i) {
                richAIAgents[i].maxSpeed = maxSpeed.Value;
                richAIAgents[i].rotationSpeed = rotationSpeed.Value;
                richAIAgents[i].enabled = true;
            }
        }

        protected override bool SetDestination(int index, Vector3 target)
        {
            richAIAgents[index].target.position = target;
            return true;
        }

        protected override Vector3 Velocity(int index)
        {
            return richAIAgents[index].Velocity;
        }

        // Disable the agents
        public override void OnEnd()
        {
            for (int i = 0; i < agents.Length; ++i) {
                richAIAgents[i].enabled = false;
                // The RVOController has to explicitly be stopped
                if (agentRVOControllers[i] != null) {
                    agentRVOControllers[i].Move(Vector3.zero);
                }
            }
        }

        public override void OnReset()
        {
            maxSpeed.Value = 1;
            rotationSpeed.Value = 360;
            agents = null;
        }
    }
}