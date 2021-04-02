using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Target Active Decision", menuName = "Scriptable Object/Pluggable AI/Decision/Target Active Decision")]
    public sealed class TargetActiveDecision : Decision
    {
        public override bool Decide(StateController stateController) => stateController.target && stateController.target.gameObject.activeSelf;
    }
}