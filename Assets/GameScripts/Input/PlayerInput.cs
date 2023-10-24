//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/GameScripts/Input/PlayerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace GameScripts
{
    public partial class @PlayerInput: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""6e045d59-e3b2-4dfe-9c41-bbf9e90271ee"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""1221389a-153d-4aba-87cf-603377a2605d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""beda39d2-0776-427a-ba1a-2b40a178d6cf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChargeAttack"",
                    ""type"": ""Button"",
                    ""id"": ""fcceabac-55b8-475a-a56f-bbfa39c01db4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause/Resume"",
                    ""type"": ""Button"",
                    ""id"": ""8e76cc6e-dc7e-42f6-aa70-02b297d66c30"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Primary Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""bf56d179-25cd-439e-8843-625a403f793a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Secondary Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""4707ddf1-9e7e-4859-8145-8e971e0fd629"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Next Interactable"",
                    ""type"": ""Button"",
                    ""id"": ""fa744c36-197b-4951-a90e-13f496b3bf56"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Previous Interactable"",
                    ""type"": ""Button"",
                    ""id"": ""f41da5e5-cc4a-411b-a6dc-a3f727a2b14a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""f1a29f0a-57f0-46fb-88bf-235392f12f91"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""ScaleVector2(x=0.2,y=0.2)"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Guard"",
                    ""type"": ""Button"",
                    ""id"": ""6864dc9a-806b-4c14-a571-75cca0ee8a0c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Switch Weapon"",
                    ""type"": ""Button"",
                    ""id"": ""f2ad83b8-d27f-41f1-9732-70cfe71f75f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Switch Offhand Weapon"",
                    ""type"": ""Button"",
                    ""id"": ""f3cf8cc0-8d07-435a-a2e2-196b0af97762"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Offhand Ability"",
                    ""type"": ""Button"",
                    ""id"": ""00616b6e-30df-45ca-a849-0fef22c79ab3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""19da8f25-1aeb-4213-b710-1e1f369a7d9a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Weapon Art"",
                    ""type"": ""Button"",
                    ""id"": ""3f6c1294-69c4-429e-ab8d-981af9bbd806"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Mouse"",
                    ""type"": ""Value"",
                    ""id"": ""c80d25c8-0a0a-40ac-8683-8d7ce03d3f50"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""78a09a5d-82dd-435f-8dbd-5d0e9c65ddde"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Directions"",
                    ""id"": ""7e185370-1913-4bae-a852-af2f456acc4a"",
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
                    ""id"": ""fd38ae07-81ca-4490-8cbd-c6649fe3e90f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8ae3c2eb-80d5-4b30-9c84-2c4eb9bd97e8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8b2b2af3-4148-4da3-8bcd-fef5a5bfa679"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""050e55de-e480-4de6-867d-18f46a8b865e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""cfc192ca-04f4-4a64-9674-c59065631c95"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause/Resume"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1916c9f0-6a9f-4c86-8af7-f20cb3da8dcf"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Primary Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47d51f0a-c713-4907-8fe0-e924bffcc3f2"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Secondary Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5220e80f-f42c-40f8-8ba6-90352f26a14a"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next Interactable"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e82ea21f-3052-490a-a890-1ccf5e2ab41b"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Previous Interactable"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""936f0667-836c-4736-8f1b-6f5d08f707ee"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76ab0ddd-2cc2-459a-8dda-f071d93b645e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Guard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7de3cb3-44c4-4b38-8089-608d800ca397"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChargeAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e2f916d-d36a-4056-8e7e-05b8899ed33c"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Switch Weapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa9b8a90-42a8-4856-b823-32e429514ef3"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Offhand Ability"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""635e8908-e302-46e3-8bab-db00edfe8f17"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f40a0e7e-889b-4b9d-a993-8adce9c61496"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Weapon Art"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d51977d-a629-4d56-96b6-c1bbae29b6a8"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Switch Offhand Weapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f0caf45c-c091-4be0-8bfd-7ec869756b6e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
            m_Player_ChargeAttack = m_Player.FindAction("ChargeAttack", throwIfNotFound: true);
            m_Player_PauseResume = m_Player.FindAction("Pause/Resume", throwIfNotFound: true);
            m_Player_PrimaryInteraction = m_Player.FindAction("Primary Interaction", throwIfNotFound: true);
            m_Player_SecondaryInteraction = m_Player.FindAction("Secondary Interaction", throwIfNotFound: true);
            m_Player_NextInteractable = m_Player.FindAction("Next Interactable", throwIfNotFound: true);
            m_Player_PreviousInteractable = m_Player.FindAction("Previous Interactable", throwIfNotFound: true);
            m_Player_Rotate = m_Player.FindAction("Rotate", throwIfNotFound: true);
            m_Player_Guard = m_Player.FindAction("Guard", throwIfNotFound: true);
            m_Player_SwitchWeapon = m_Player.FindAction("Switch Weapon", throwIfNotFound: true);
            m_Player_SwitchOffhandWeapon = m_Player.FindAction("Switch Offhand Weapon", throwIfNotFound: true);
            m_Player_OffhandAbility = m_Player.FindAction("Offhand Ability", throwIfNotFound: true);
            m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
            m_Player_WeaponArt = m_Player.FindAction("Weapon Art", throwIfNotFound: true);
            m_Player_Mouse = m_Player.FindAction("Mouse", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Player
        private readonly InputActionMap m_Player;
        private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_Attack;
        private readonly InputAction m_Player_ChargeAttack;
        private readonly InputAction m_Player_PauseResume;
        private readonly InputAction m_Player_PrimaryInteraction;
        private readonly InputAction m_Player_SecondaryInteraction;
        private readonly InputAction m_Player_NextInteractable;
        private readonly InputAction m_Player_PreviousInteractable;
        private readonly InputAction m_Player_Rotate;
        private readonly InputAction m_Player_Guard;
        private readonly InputAction m_Player_SwitchWeapon;
        private readonly InputAction m_Player_SwitchOffhandWeapon;
        private readonly InputAction m_Player_OffhandAbility;
        private readonly InputAction m_Player_Dash;
        private readonly InputAction m_Player_WeaponArt;
        private readonly InputAction m_Player_Mouse;
        public struct PlayerActions
        {
            private @PlayerInput m_Wrapper;
            public PlayerActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @Attack => m_Wrapper.m_Player_Attack;
            public InputAction @ChargeAttack => m_Wrapper.m_Player_ChargeAttack;
            public InputAction @PauseResume => m_Wrapper.m_Player_PauseResume;
            public InputAction @PrimaryInteraction => m_Wrapper.m_Player_PrimaryInteraction;
            public InputAction @SecondaryInteraction => m_Wrapper.m_Player_SecondaryInteraction;
            public InputAction @NextInteractable => m_Wrapper.m_Player_NextInteractable;
            public InputAction @PreviousInteractable => m_Wrapper.m_Player_PreviousInteractable;
            public InputAction @Rotate => m_Wrapper.m_Player_Rotate;
            public InputAction @Guard => m_Wrapper.m_Player_Guard;
            public InputAction @SwitchWeapon => m_Wrapper.m_Player_SwitchWeapon;
            public InputAction @SwitchOffhandWeapon => m_Wrapper.m_Player_SwitchOffhandWeapon;
            public InputAction @OffhandAbility => m_Wrapper.m_Player_OffhandAbility;
            public InputAction @Dash => m_Wrapper.m_Player_Dash;
            public InputAction @WeaponArt => m_Wrapper.m_Player_WeaponArt;
            public InputAction @Mouse => m_Wrapper.m_Player_Mouse;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void AddCallbacks(IPlayerActions instance)
            {
                if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @ChargeAttack.started += instance.OnChargeAttack;
                @ChargeAttack.performed += instance.OnChargeAttack;
                @ChargeAttack.canceled += instance.OnChargeAttack;
                @PauseResume.started += instance.OnPauseResume;
                @PauseResume.performed += instance.OnPauseResume;
                @PauseResume.canceled += instance.OnPauseResume;
                @PrimaryInteraction.started += instance.OnPrimaryInteraction;
                @PrimaryInteraction.performed += instance.OnPrimaryInteraction;
                @PrimaryInteraction.canceled += instance.OnPrimaryInteraction;
                @SecondaryInteraction.started += instance.OnSecondaryInteraction;
                @SecondaryInteraction.performed += instance.OnSecondaryInteraction;
                @SecondaryInteraction.canceled += instance.OnSecondaryInteraction;
                @NextInteractable.started += instance.OnNextInteractable;
                @NextInteractable.performed += instance.OnNextInteractable;
                @NextInteractable.canceled += instance.OnNextInteractable;
                @PreviousInteractable.started += instance.OnPreviousInteractable;
                @PreviousInteractable.performed += instance.OnPreviousInteractable;
                @PreviousInteractable.canceled += instance.OnPreviousInteractable;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Guard.started += instance.OnGuard;
                @Guard.performed += instance.OnGuard;
                @Guard.canceled += instance.OnGuard;
                @SwitchWeapon.started += instance.OnSwitchWeapon;
                @SwitchWeapon.performed += instance.OnSwitchWeapon;
                @SwitchWeapon.canceled += instance.OnSwitchWeapon;
                @SwitchOffhandWeapon.started += instance.OnSwitchOffhandWeapon;
                @SwitchOffhandWeapon.performed += instance.OnSwitchOffhandWeapon;
                @SwitchOffhandWeapon.canceled += instance.OnSwitchOffhandWeapon;
                @OffhandAbility.started += instance.OnOffhandAbility;
                @OffhandAbility.performed += instance.OnOffhandAbility;
                @OffhandAbility.canceled += instance.OnOffhandAbility;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @WeaponArt.started += instance.OnWeaponArt;
                @WeaponArt.performed += instance.OnWeaponArt;
                @WeaponArt.canceled += instance.OnWeaponArt;
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
            }

            private void UnregisterCallbacks(IPlayerActions instance)
            {
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @Attack.started -= instance.OnAttack;
                @Attack.performed -= instance.OnAttack;
                @Attack.canceled -= instance.OnAttack;
                @ChargeAttack.started -= instance.OnChargeAttack;
                @ChargeAttack.performed -= instance.OnChargeAttack;
                @ChargeAttack.canceled -= instance.OnChargeAttack;
                @PauseResume.started -= instance.OnPauseResume;
                @PauseResume.performed -= instance.OnPauseResume;
                @PauseResume.canceled -= instance.OnPauseResume;
                @PrimaryInteraction.started -= instance.OnPrimaryInteraction;
                @PrimaryInteraction.performed -= instance.OnPrimaryInteraction;
                @PrimaryInteraction.canceled -= instance.OnPrimaryInteraction;
                @SecondaryInteraction.started -= instance.OnSecondaryInteraction;
                @SecondaryInteraction.performed -= instance.OnSecondaryInteraction;
                @SecondaryInteraction.canceled -= instance.OnSecondaryInteraction;
                @NextInteractable.started -= instance.OnNextInteractable;
                @NextInteractable.performed -= instance.OnNextInteractable;
                @NextInteractable.canceled -= instance.OnNextInteractable;
                @PreviousInteractable.started -= instance.OnPreviousInteractable;
                @PreviousInteractable.performed -= instance.OnPreviousInteractable;
                @PreviousInteractable.canceled -= instance.OnPreviousInteractable;
                @Rotate.started -= instance.OnRotate;
                @Rotate.performed -= instance.OnRotate;
                @Rotate.canceled -= instance.OnRotate;
                @Guard.started -= instance.OnGuard;
                @Guard.performed -= instance.OnGuard;
                @Guard.canceled -= instance.OnGuard;
                @SwitchWeapon.started -= instance.OnSwitchWeapon;
                @SwitchWeapon.performed -= instance.OnSwitchWeapon;
                @SwitchWeapon.canceled -= instance.OnSwitchWeapon;
                @SwitchOffhandWeapon.started -= instance.OnSwitchOffhandWeapon;
                @SwitchOffhandWeapon.performed -= instance.OnSwitchOffhandWeapon;
                @SwitchOffhandWeapon.canceled -= instance.OnSwitchOffhandWeapon;
                @OffhandAbility.started -= instance.OnOffhandAbility;
                @OffhandAbility.performed -= instance.OnOffhandAbility;
                @OffhandAbility.canceled -= instance.OnOffhandAbility;
                @Dash.started -= instance.OnDash;
                @Dash.performed -= instance.OnDash;
                @Dash.canceled -= instance.OnDash;
                @WeaponArt.started -= instance.OnWeaponArt;
                @WeaponArt.performed -= instance.OnWeaponArt;
                @WeaponArt.canceled -= instance.OnWeaponArt;
                @Mouse.started -= instance.OnMouse;
                @Mouse.performed -= instance.OnMouse;
                @Mouse.canceled -= instance.OnMouse;
            }

            public void RemoveCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IPlayerActions instance)
            {
                foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public PlayerActions @Player => new PlayerActions(this);
        public interface IPlayerActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
            void OnChargeAttack(InputAction.CallbackContext context);
            void OnPauseResume(InputAction.CallbackContext context);
            void OnPrimaryInteraction(InputAction.CallbackContext context);
            void OnSecondaryInteraction(InputAction.CallbackContext context);
            void OnNextInteractable(InputAction.CallbackContext context);
            void OnPreviousInteractable(InputAction.CallbackContext context);
            void OnRotate(InputAction.CallbackContext context);
            void OnGuard(InputAction.CallbackContext context);
            void OnSwitchWeapon(InputAction.CallbackContext context);
            void OnSwitchOffhandWeapon(InputAction.CallbackContext context);
            void OnOffhandAbility(InputAction.CallbackContext context);
            void OnDash(InputAction.CallbackContext context);
            void OnWeaponArt(InputAction.CallbackContext context);
            void OnMouse(InputAction.CallbackContext context);
        }
    }
}
