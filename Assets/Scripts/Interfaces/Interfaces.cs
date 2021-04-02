using Assets.Scripts.Abilities;

namespace Assets.Scripts.Interfaces
{
    public interface IDestructible
    {
        void Destroy(float time);
    }

    public interface IAttackable
    {
        void Attack(AttackType attackType);
    }

    public interface IDamageable
    {
        void Damaged(AttackType attackType);
    }
}