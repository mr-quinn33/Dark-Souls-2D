using Assets.Scripts.Abilities;
using Assets.Scripts.Characters;
using Assets.Scripts.Equipments;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.CharacterControllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class GuardController : MonoBehaviour, IDamageable
    {
        #region Fields
        public Shield shield;

        private Character character;
        private Animator animator;
        private BoxCollider2D shieldCollider;
        private bool isGuarding;
        #endregion

        private void Start()
        {
            character = GetComponentInParent<CharacterController2D>().character;
            Debug.Assert(animator = transform.parent.GetComponent<Animator>(), $"No animator component attached to {transform.parent.name}!");
        }

        #region Methods
        public void Damaged(AttackType attackType)
        {
            if (attackType is MagicAttack)
            {
                character.DecreaseHealth((int)((attackType.power - character.MagicalDefence) * (1f - shield.magicalDamageReduction)));
            }
            else
            {
                character.DecreaseHealth((int)((attackType.power - character.PhysicalDefence) * (1f - shield.physicalDamageReduction)));
            }
            character.DecreaseStaminaAsync(shield.staminaCost * (1f - shield.staminaCostReduction));
        }

        public void GuardCanceled()
        {
            if (isGuarding)
            {
                Guard(isGuarding = !isGuarding);
                character.OnStaminaExhausted -= GuardBreak;
            }
        }

        private protected void GuardPerformed()
        {
            if (character.Stamina > 0f)
            {
                Guard(isGuarding = !isGuarding);
                character.OnStaminaExhausted += GuardBreak;
            }
        }

        private void GuardBreak() => animator.SetTrigger("GuardBreak");

        private void Guard(bool isGuarding)
        {
            animator.SetBool("Guard", isGuarding);
            if (isGuarding)
            {
                character.DecreaseStaminaRegen(shield.staminaRegenPenalty);
                shieldCollider = gameObject.AddComponent<BoxCollider2D>();
                switch (character.Direction)
                {
                    case Character.Directions.Down:
                        shieldCollider.size = new Vector2(shield.size.y, shield.size.x);
                        shieldCollider.offset = new Vector2(default, -shield.offset);
                        break;
                    case Character.Directions.Up:
                        shieldCollider.size = new Vector2(shield.size.y, shield.size.x);
                        shieldCollider.offset = new Vector2(default, shield.offset);
                        break;
                    case Character.Directions.Left:
                        shieldCollider.size = new Vector2(shield.size.x, shield.size.y);
                        shieldCollider.offset = new Vector2(-shield.offset, default);
                        break;
                    case Character.Directions.Right:
                        shieldCollider.size = new Vector2(shield.size.x, shield.size.y);
                        shieldCollider.offset = new Vector2(shield.offset, default);
                        break;
                    default:
                        Destroy(shieldCollider);
                        break;
                }
            }
            else
            {
                character.IncreaseStaminaRegen(shield.staminaRegenPenalty);
                Destroy(shieldCollider);
            }
        }
        #endregion
    }
}