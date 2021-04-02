// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input System/Input Actions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
    public class @InputActions : IInputActionCollection, IDisposable
    {
        private static @InputActions instance;
        public static @InputActions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new @InputActions();
                }
                return instance;
            }
        }
        public InputActionAsset Asset { get; }
        public @InputActions()
        {
            Asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input Actions"",
    ""maps"": [
        {
            ""name"": ""Character Control"",
            ""id"": ""b17a9f36-afa9-4939-9a53-c6566105ee51"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""c4d9b1ab-cef6-4240-96f5-375cc7cd9916"",
                    ""expectedControlType"": ""Key"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""09c1fc26-8250-4c6e-bd35-8bf86a0692aa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Guard"",
                    ""type"": ""Button"",
                    ""id"": ""fcbfcf6b-cdef-43f4-9eb5-4b1d9f3893d2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""d2c5c536-85ba-42e0-a830-08fa384fff67"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""687cd3dc-dac2-428b-8ce0-2fb993f4d7ad"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""d5c5a335-9d9c-401b-af35-eab8399b3958"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""846ecdfc-66a7-4f78-9163-21a9a69d4da0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0a995e44-c621-408f-93aa-5e7b183c1ef1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""94ac086f-9563-4a7c-aaea-4c0baf48d8cf"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0fd3eaba-7e29-471f-9a99-97888d7736b3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""edb84dd2-e9fe-477d-8864-bb5c30e08c3d"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9c35c999-4c00-4c5a-be86-030c239ce1de"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Mouse"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""288c74f0-4ecc-4bf5-946e-15b4f91bb1e8"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Guard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4639af2c-b8c8-439f-86d6-cee58f1460ac"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Mouse"",
                    ""action"": ""Guard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e457051-78eb-4a2e-b5b1-9396695b2316"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c191aaf0-c53b-48e1-b3cd-ab7e37bb7747"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Mouse"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard Mouse"",
            ""bindingGroup"": ""Keyboard Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Character Control
            m_CharacterControl = Asset.FindActionMap("Character Control", throwIfNotFound: true);
            m_CharacterControl_Move = m_CharacterControl.FindAction("Move", throwIfNotFound: true);
            m_CharacterControl_Attack = m_CharacterControl.FindAction("Attack", throwIfNotFound: true);
            m_CharacterControl_Guard = m_CharacterControl.FindAction("Guard", throwIfNotFound: true);
            m_CharacterControl_Run = m_CharacterControl.FindAction("Run", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(Asset);
            instance = null;
        }

        public InputBinding? bindingMask
        {
            get => Asset.bindingMask;
            set => Asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => Asset.devices;
            set => Asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => Asset.controlSchemes;

        public bool Contains(InputAction action) => Asset.Contains(action);

        public IEnumerator<InputAction> GetEnumerator() => Asset.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Enable() => Asset.Enable();

        public void Disable() => Asset.Disable();

        // Character Control
        private readonly InputActionMap m_CharacterControl;
        private ICharacterControlActions m_CharacterControlActionsCallbackInterface;
        private readonly InputAction m_CharacterControl_Move;
        private readonly InputAction m_CharacterControl_Attack;
        private readonly InputAction m_CharacterControl_Guard;
        private readonly InputAction m_CharacterControl_Run;
        public struct CharacterControlActions
        {
            private @InputActions m_Wrapper;
            public CharacterControlActions(@InputActions wrapper) => m_Wrapper = wrapper;
            public InputAction @Move => m_Wrapper.m_CharacterControl_Move;
            public InputAction @Attack => m_Wrapper.m_CharacterControl_Attack;
            public InputAction @Guard => m_Wrapper.m_CharacterControl_Guard;
            public InputAction @Run => m_Wrapper.m_CharacterControl_Run;
            public InputActionMap Get() => m_Wrapper.m_CharacterControl;
            public void Enable() => Get().Enable();
            public void Disable() => Get().Disable();
            public bool Enabled => Get().enabled;
            public static implicit operator InputActionMap(CharacterControlActions set) => set.Get();
            public void SetCallbacks(ICharacterControlActions instance)
            {
                if (m_Wrapper.m_CharacterControlActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnMove;
                    @Attack.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnAttack;
                    @Attack.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnAttack;
                    @Attack.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnAttack;
                    @Guard.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnGuard;
                    @Guard.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnGuard;
                    @Guard.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnGuard;
                    @Run.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnRun;
                    @Run.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnRun;
                    @Run.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnRun;
                }
                m_Wrapper.m_CharacterControlActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Attack.started += instance.OnAttack;
                    @Attack.performed += instance.OnAttack;
                    @Attack.canceled += instance.OnAttack;
                    @Guard.started += instance.OnGuard;
                    @Guard.performed += instance.OnGuard;
                    @Guard.canceled += instance.OnGuard;
                    @Run.started += instance.OnRun;
                    @Run.performed += instance.OnRun;
                    @Run.canceled += instance.OnRun;
                }
            }
        }
        public CharacterControlActions @CharacterControl => new CharacterControlActions(this);
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1)
                {
                    m_GamepadSchemeIndex = Asset.FindControlSchemeIndex("Gamepad");
                }

                return Asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1)
                {
                    m_KeyboardMouseSchemeIndex = Asset.FindControlSchemeIndex("Keyboard Mouse");
                }

                return Asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        public interface ICharacterControlActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
            void OnGuard(InputAction.CallbackContext context);
            void OnRun(InputAction.CallbackContext context);
        }
    }
}
