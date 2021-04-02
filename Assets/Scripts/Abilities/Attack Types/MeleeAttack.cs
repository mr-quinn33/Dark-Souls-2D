using UnityEngine;

namespace Assets.Scripts.Abilities
{
    [CreateAssetMenu(fileName = "New Melee Attack", menuName = "Scriptable Object/Attack Type/Melee Attack")]
    public sealed class MeleeAttack : AttackType
    {
        public Vector2 size;
        public float angle;
        public float force;
    }
}
