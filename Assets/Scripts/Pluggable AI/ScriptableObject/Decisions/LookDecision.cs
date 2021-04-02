using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Look Decision", menuName = "Scriptable Object/Pluggable AI/Decision/Look Decision")]
    public sealed class LookDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(stateController.transform.position, stateController.vision.radius, stateController.Character.Vector, float.PositiveInfinity, stateController.vision.targetLayer);
            if (raycastHit2D && !Physics2D.Linecast(stateController.transform.position, raycastHit2D.transform.position, stateController.vision.obstacleLayer))
            {
                if (!stateController.target)
                {
                    stateController.target = raycastHit2D.transform;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}