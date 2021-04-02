using Assets.Scripts.Abilities;
using Assets.Scripts.Characters;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Utilities;
using System;
using UnityEngine;

namespace Assets.Scripts.CharacterControllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AttackController : MonoBehaviour, IAttackable
    {
        #region Fields
        public AttackType attackType;

        private protected Character character;
        private protected new Rigidbody2D rigidbody2D;
        private protected new Collider2D collider2D;

        private Animator animator;
        private Coroutine coroutine;
        private bool isCombo;
        #endregion

        #region Callbacks
        private protected virtual void Start()
        {
            character = GetComponentInParent<CharacterController2D>().character;
            Debug.Assert(animator = transform.parent.GetComponent<Animator>(), $"No animator component attached to {transform.parent.name}!");
            Debug.Assert(rigidbody2D = transform.parent.GetComponent<Rigidbody2D>(), $"No rigidbody2D component attached to {transform.parent.name}!");
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.collider.TryGetComponent(out IDamageable damageable))
            {
                if (collision2D.collider.TryGetComponent(out Animator animator))
                {
                    animator.SetTrigger("Damaged");
                }
                damageable.Damaged(attackType);
            }
        }
        #endregion

        #region Methods
        public void ResetCombo() => isCombo = false;

        public void Attack(AttackType attackType)
        {
            if (attackType is MeleeAttack)
            {
                if (character.Stamina > 0f && coroutine == null)
                {
                    animator.SetTrigger("MeleeAttack");
                    coroutine = StartCoroutine(CoroutineUtility.WaitForSecondsFunc(attackType.startTime, () =>
                    {
                        MeleeAttack(isCombo);
                        isCombo = !isCombo;
                        character.DecreaseStaminaAsync(attackType.staminaCost);
                        return CoroutineUtility.WaitForSecondsAction(attackType.duration, () =>
                        {
                            Destroy(collider2D);
                            StopCoroutine(coroutine);
                            coroutine = null;
                        });
                    }));
                }
            }
            else
            {
                if (attackType is RangedAttack)
                {
                    RangedAttack();
                    character.DecreaseStaminaAsync(attackType.staminaCost);
                }
                else
                {
                    if (attackType is MagicAttack)
                    {
                        MagicAttack();
                        character.DecreaseMana(attackType.manaCost);
                    }
                    else
                    {
                        throw new InvalidCastException($"Invalid attack type {attackType}!");
                    }
                }
            }
        }

        private protected abstract void MeleeAttack(bool isCombo);

        private protected abstract void RangedAttack();

        private protected abstract void MagicAttack();
        #endregion
    }
}