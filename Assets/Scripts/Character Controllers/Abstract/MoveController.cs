using Assets.Scripts.Characters;
using UnityEngine;

namespace Assets.Scripts.CharacterControllers
{
    [RequireComponent(typeof(CharacterController2D), typeof(Rigidbody2D))]
    public abstract class MoveController : MonoBehaviour
    {
        private protected new Rigidbody2D rigidbody2D;

        private Character character;
        private Animator animator;

        private protected virtual void Start()
        {
            character = GetComponent<CharacterController2D>().character;
            animator = GetComponent<Animator>();
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        #region Methods
        private protected virtual void Move(Vector2 vector2)
        {
            if (vector2 == default)
            {
                animator.SetFloat("Speed", default);
            }
            else
            {
                character.Vector = vector2;
                animator.SetFloat("YAxis", vector2.y);
                animator.SetFloat("XAxis", vector2.x);
                animator.SetFloat("Speed", vector2.sqrMagnitude);
            }
        }
        #endregion
    }
}
