using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

namespace Pathfinding
{
    // Subclass the RichAI class to provide common functions for the Movement agents
    public class RichAIAgent : RichAI
    {
        private bool targetReached = false;
        public bool TargetReached { get { return targetReached; } }

        // Has the path been created and is valid?
        public bool PathCalculated()
        {
            return !waitingForPathCalc;
        }

        // The target has not been reached when the path is updated
        public override void UpdatePath()
        {
            base.UpdatePath();

            targetReached = false;
        }

        // The target has been reached
        protected override void OnTargetReached()
        {
            base.OnTargetReached();

            targetReached = true;
        }
    }
}