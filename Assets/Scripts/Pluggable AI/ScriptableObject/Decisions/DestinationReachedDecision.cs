using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Destination Reached Decision", menuName = "Scriptable Object/Pluggable AI/Decision/Destination Reached Decision")]
    public sealed class DestinationReachedDecision : Decision
    {
        public override bool Decide(StateController stateController) => stateController.NavMeshAgent.remainingDistance < stateController.NavMeshAgent.stoppingDistance;
    }
}