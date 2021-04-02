using Assets.Scripts.CharacterControllers;
using UnityEngine;

namespace Assets.Scripts.StateMachineBehaviours
{
    internal sealed class ToggleGuardController : StateMachineBehaviour
    {
        private bool flag;
        private GuardController guardController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if ((guardController = animator.GetComponentInChildren<GuardController>()) && (flag = guardController.enabled))
            {
                guardController.GuardCanceled();
                guardController.enabled = false;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (guardController && flag)
            {
                guardController.enabled = true;
            }
        }
    }
}
