using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Within Range Decision", menuName = "Scriptable Object/Pluggable AI/Decision/Within Range Decision")]
    public sealed class WithinRangeDecision : Decision
    {
        public override bool Decide(StateController stateController) => stateController.target && (stateController.transform.position - stateController.target.position).sqrMagnitude < stateController.AttackController.attackType.SquaredRange;
    }
}