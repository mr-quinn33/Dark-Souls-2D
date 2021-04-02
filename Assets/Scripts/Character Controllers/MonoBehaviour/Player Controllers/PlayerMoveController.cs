using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.CharacterControllers
{
    public sealed class PlayerMoveController : MoveController
    {
        #region Fields
        [SerializeField] [Range(0f, 10f)] private float moveSpeed;
        [SerializeField] [Range(0f, 5f)] private float speedBoost;

        private InputAction move;
        private InputAction run;
        #endregion

        #region Callbacks
        private void Awake()
        {
            move = InputActions.Instance.CharacterControl.Move;
            run = InputActions.Instance.CharacterControl.Run;
            run.performed += context => moveSpeed += speedBoost;
            run.canceled += context => moveSpeed -= speedBoost;
        }

        private void OnEnable()
        {
            move.Enable();
            run.Enable();
        }

        private void FixedUpdate() => Move(move.ReadValue<Vector2>() * moveSpeed);

        private void OnDisable()
        {
            move.Disable();
            run.Disable();
            rigidbody2D.velocity = default;
        }

        private void OnDestroy() => InputActions.Instance.Dispose();
        #endregion

        private protected override void Move(Vector2 vector2)
        {
            base.Move(vector2);
            rigidbody2D.velocity = vector2;
        }
    }
}
