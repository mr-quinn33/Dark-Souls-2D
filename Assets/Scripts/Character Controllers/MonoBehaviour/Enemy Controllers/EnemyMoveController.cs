using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CharacterControllers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyMoveController : MoveController
    {
        private NavMeshAgent navMeshAgent;

        private protected override void Start()
        {
            base.Start();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.updateRotation = false;
        }

        private void FixedUpdate() => Move(navMeshAgent.velocity);
    }
}
