using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Wander Action", menuName = "Scriptable Object/Pluggable AI/Action/Wander Action")]
    public sealed class WanderAction : Action
    {
        public float wanderScale;
        public float waitSpan;

        public override void Act(StateController stateController)
        {
            if (!stateController.NavMeshAgent.pathPending && stateController.NavMeshAgent.remainingDistance < stateController.NavMeshAgent.stoppingDistance && stateController.IsTimeElapsed(Random.value * waitSpan))
            {
                _ = stateController.NavMeshAgent.SetDestination((Vector2)stateController.transform.position + (Random.insideUnitCircle * wanderScale));
            }
        }
    }
}