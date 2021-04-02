using Assets.Scripts.CharacterControllers;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.StateMachineBehaviours
{
    internal sealed class ToggleMoveController : StateMachineBehaviour
    {
        private bool flag;
        private MoveController moveController;
        private NavMeshAgent navMeshAgent;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if ((moveController = animator.GetComponent<MoveController>()) && (flag = moveController.enabled))
            {
                moveController.enabled = false;
            }
            if ((navMeshAgent = animator.GetComponent<NavMeshAgent>()) && navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (moveController && flag)
            {
                moveController.enabled = true;
            }
            if (navMeshAgent && navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = false;
            }
        }
    }
}
