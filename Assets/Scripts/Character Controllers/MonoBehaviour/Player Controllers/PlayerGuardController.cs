using UnityEngine.InputSystem;

namespace Assets.Scripts.CharacterControllers
{
    public sealed class PlayerGuardController : GuardController
    {
        private InputAction guard;

        #region Callbacks
        private void Awake()
        {
            guard = InputActions.Instance.CharacterControl.Guard;
            guard.performed += context => GuardPerformed();
            guard.canceled += context => GuardCanceled();
        }

        private void OnEnable() => guard.Enable();

        private void OnDisable() => guard.Disable();

        private void OnDestroy() => InputActions.Instance.Dispose();
        #endregion
    }
}