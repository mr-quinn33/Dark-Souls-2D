using Assets.Scripts.CharacterControllers;
using UnityEngine;

namespace Assets.Scripts.StateMachineBehaviours
{
    internal sealed class ResetAttackCombo : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AttackController attackController = animator.GetComponentInChildren<AttackController>();
            if (attackController)
            {
                attackController.ResetCombo();
            }
        }
    }
}
