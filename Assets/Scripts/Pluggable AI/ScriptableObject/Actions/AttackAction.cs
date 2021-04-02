using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Attack Action", menuName = "Scriptable Object/Pluggable AI/Action/Attack Action")]
    public sealed class AttackAction : Action
    {
        public override void Act(StateController stateController)
        {
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(stateController.transform.position, stateController.vision.radius, stateController.Character.Vector, float.PositiveInfinity, stateController.vision.targetLayer);
            if (raycastHit2D && stateController.IsTimeElapsed(stateController.AttackController.attackType.TotalTime))
            {
                stateController.AttackController.Attack(stateController.AttackController.attackType);
            }
        }
    }
}