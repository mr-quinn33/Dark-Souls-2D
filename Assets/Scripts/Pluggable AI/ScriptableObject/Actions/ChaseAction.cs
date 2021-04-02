using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Chase Action", menuName = "Scriptable Object/Pluggable AI/Action/Chase Action")]
    public sealed class ChaseAction : Action
    {
        public override void Act(StateController stateController)
        {
            if (stateController.target)
            {
                _ = stateController.NavMeshAgent.SetDestination(stateController.target.position);
            }
        }
    }
}