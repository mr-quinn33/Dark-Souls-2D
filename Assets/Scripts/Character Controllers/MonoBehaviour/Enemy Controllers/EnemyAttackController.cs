using Assets.Scripts.Abilities;
using Assets.Scripts.Characters;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.CharacterControllers
{
    public sealed class EnemyAttackController : AttackController
    {
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

        private protected override void MagicAttack() { }

        private protected override void RangedAttack() { }
    }
}
