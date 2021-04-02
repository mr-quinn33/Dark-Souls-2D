using Assets.Scripts.CharacterControllers;
using UnityEngine;

namespace Assets.Scripts.StateMachineBehaviours
{
    internal sealed class ToggleAttackController : StateMachineBehaviour
    {
        private bool flag;
        private AttackController attackController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if ((attackController = animator.GetComponentInChildren<AttackController>()) && (flag = attackController.enabled))
            {
                foreach (Collider2D collider2D in attackController.GetComponents<Collider2D>())
                {
                    Destroy(collider2D);
                }
                attackController.enabled = false;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (attackController && flag)
            {
                attackController.enabled = true;
            }
        }
    }
}
