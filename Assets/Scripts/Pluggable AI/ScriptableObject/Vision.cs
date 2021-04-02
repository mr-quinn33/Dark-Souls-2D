using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New Vision", menuName = "Scriptable Object/Pluggable AI/Vision")]
    public sealed class Vision : ScriptableObject
    {
        public LayerMask targetLayer;
        public LayerMask obstacleLayer;
        public float radius;
    }
}