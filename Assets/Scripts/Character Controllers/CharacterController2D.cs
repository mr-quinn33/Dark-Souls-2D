using Assets.Scripts.Abilities;
using Assets.Scripts.Characters;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CharacterControllers
{
    [RequireComponent(typeof(Animator))]
    public sealed class CharacterController2D : MonoBehaviour, IDamageable
    {
        public Character character;

        private void Start()
        {
            if (character is Enemy)
            {
                character = character.Clone(ScriptableObject.CreateInstance<Enemy>());
            }
            character.OnDestroy += Destroy;
        }

        private void OnDisable() => character.OnDestroy -= Destroy;

        public void Damaged(AttackType attackType)
        {
            if (attackType is MagicAttack)
            {
                character.DecreaseHealth(attackType.power - character.MagicalDefence);
            }
            else
            {
                character.DecreaseHealth(attackType.power - character.PhysicalDefence);
            }
        }

        private Animator Destroy(float time)
        {
            enabled = false;
            foreach (Rigidbody2D rigidbody2D in GetComponentsInChildren<Rigidbody2D>())
            {
                rigidbody2D.simulated = false;
            }
            if (TryGetComponent(out NavMeshAgent navMeshAgent))
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false;
            }
            Destroy(gameObject, time);
            Destroy(this, time);
            return GetComponent<Animator>();
        }

        private void OnApplicationQuit()
        {
            if (character is Player)
            {
                Player player = character as Player;
                player.SetPosition(transform.position);
                SaveUtility.Save(player);
            }
        }
    }
}