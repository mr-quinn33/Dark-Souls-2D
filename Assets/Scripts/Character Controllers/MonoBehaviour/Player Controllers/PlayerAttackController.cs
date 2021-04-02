using Assets.Scripts.Abilities;
using Assets.Scripts.Characters;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.CharacterControllers
{
    public sealed class PlayerAttackController : AttackController
    {
        private InputAction attack;

        #region Callbacks
        private void Awake() => attack = InputActions.Instance.CharacterControl.Attack;

        private void OnEnable() => attack.Enable();

        private protected override void Start()
        {
            base.Start();
            attack.performed += callbackContext => Attack(attackType);
        }

        private void OnDisable() => attack.Disable();

        private void OnDestroy() => InputActions.Instance.Dispose();
        #endregion

        #region Methods
        private protected override void MeleeAttack(bool isCombo)
        {
            MeleeAttack meleeAttack = attackType as MeleeAttack;
            BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.size = meleeAttack.size;
            boxCollider2D.offset = new Vector2(meleeAttack.size.x * 0.5f, default);
            collider2D = boxCollider2D;
            switch (character.Direction)
            {
                case Character.Directions.Down:
                    transform.rotation = Quaternion.AngleAxis(270f + ((isCombo ? 1 : -1) * meleeAttack.angle * 0.5f), Vector3.forward);
                    rigidbody2D.AddForce(Vector2.down * meleeAttack.force);
                    break;
                case Character.Directions.Up:
                    transform.rotation = Quaternion.AngleAxis(90f + ((isCombo ? 1 : -1) * meleeAttack.angle * 0.5f), Vector3.forward);
                    rigidbody2D.AddForce(Vector2.up * meleeAttack.force);
                    break;
                case Character.Directions.Left:
                    transform.rotation = Quaternion.AngleAxis(180f + ((isCombo ? 1 : -1) * meleeAttack.angle * 0.5f), Vector3.forward);
                    rigidbody2D.AddForce(Vector2.left * meleeAttack.force);
                    break;
                case Character.Directions.Right:
                    transform.rotation = Quaternion.AngleAxis((isCombo ? 1 : -1) * meleeAttack.angle * 0.5f, Vector3.forward);
                    rigidbody2D.AddForce(Vector2.right * meleeAttack.force);
                    break;
                default:
                    Destroy(collider2D);
                    return;
            }
            _ = StartCoroutine(transform.Rotate(isCombo, meleeAttack.angle, meleeAttack.duration));
        }

        private protected override void RangedAttack() { }

        private protected override void MagicAttack() { }
        #endregion
    }
}
