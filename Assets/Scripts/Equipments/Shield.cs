using UnityEngine;

namespace Assets.Scripts.Equipments
{
    [CreateAssetMenu(fileName = "New Shield", menuName = "Scriptable Object/Equipment/Shield")]
    public class Shield : ScriptableObject
    {
        [Header("Shield Configuration")]
        public Vector2 size;
        public float offset;
        [Range(0f, 1f)] public float staminaCostReduction;
        [Range(0f, 1f)] public float physicalDamageReduction;
        [Range(0f, 1f)] public float magicalDamageReduction;

        [Header("Stamina Configuration")]
        public float staminaCost;
        public float staminaRegenPenalty;
    }
}