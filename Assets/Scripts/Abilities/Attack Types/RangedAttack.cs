using UnityEngine;

namespace Assets.Scripts.Abilities
{
    [CreateAssetMenu(fileName = "New Ranged Attack", menuName = "Scriptable Object/Attack Type/Ranged Attack")]
    public sealed class RangedAttack : AttackType
    {
        public byte ammoCost;
    }
}
