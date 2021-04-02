using UnityEngine;

namespace Assets.Scripts.Abilities
{
    public abstract class AttackType : ScriptableObject
    {
        public int power;
        public int manaCost;
        public float range;
        public float staminaCost;
        public float startTime;
        public float duration;

        public float TotalTime => startTime + duration;

        public float SquaredRange => range * range;
    }
}
