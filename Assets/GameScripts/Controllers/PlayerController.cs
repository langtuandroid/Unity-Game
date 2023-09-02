using UnityEngine;
using UnityEngine.InputSystem;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Interaction;
using LobsterFramework.EntitySystem;
using GameScripts.Abilities;
using GameScripts.Interaction;
using LobsterFramework;

namespace GameScripts.InputControl
{
    [RequireComponent(typeof(GeneralInteractor))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private float cameraDistance;
        [SerializeField] private VarBool gamePause;

        [Header("Event Channels")]
        [SerializeField] private VoidEventChannel gameEndChannel;
        [SerializeField] private VoidEventChannel playerDeathChannel;
        [SerializeField] private VoidEventChannel playerRespawnChennel;

        [Header("Components")]
        [SerializeField] private AbilityRunner abilityRunner;
        [SerializeField] private GeneralInteractor interactor;
        [SerializeField] private WeaponWielder weaponWielder;
        private Entity player;
        private MovementController moveControl;


        [Header("Inputs")]
        [SerializeField] private InputActionReference rotate;
        [SerializeField] private InputActionReference move;
        [SerializeField] private float rotateAcceleration;
        [SerializeField] private float mouseSensitivity;
        private float rotateVelocity;

        private Transform _transform;

        public void Start()
        {
            playerRespawnChennel.OnEventRaised += RespawnPlayer;
            _transform = GetComponent<Transform>();
            moveControl = GetComponent<MovementController>();
            player = GetComponent<Entity>();
        }

        private void FixedUpdate()
        {
            if (!gamePause.Value)
            {
                Vector2 input = rotate.action.ReadValue<Vector2>();
                if (input.x != 0)
                {
                    float delta = input.x * Time.deltaTime * mouseSensitivity; 
                    rotateVelocity = Mathf.MoveTowards(rotateVelocity, -delta, rotateAcceleration * Time.deltaTime);
                    rotateVelocity = Mathf.Clamp(rotateVelocity, -moveControl.RotateSpeed, moveControl.RotateSpeed);
                }
                else { 
                    rotateVelocity = Mathf.MoveTowards(rotateVelocity, 0, rotateAcceleration * Time.deltaTime);
                }
                moveControl.RotateByDegrees(rotateVelocity);
            }
        }

        private void Update()
        {
            moveControl.Move(move.action.ReadValue<Vector2>());
        }

        private void OnDisable()
        {
            if (player.IsDead)
            {
                playerDeathChannel.RaiseEvent();
            }
        }

        private void OnDestroy()
        {
            playerRespawnChennel.OnEventRaised -= RespawnPlayer;
        }

        private void RespawnPlayer()
        {
            player.ResetStatus();
        }

        public void WeaponLightAttack(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                if (abilityRunner.IsAbilityReady<Boost>())
                {
                    abilityRunner.EnqueueAbilitiesInJoint<LightWeaponAttack, Boost>();
                }
                else {
                    abilityRunner.EnqueueAbility<LightWeaponAttack>();
                }
            }
        }

        public void WeaponHeavyAttack(InputAction.CallbackContext context) {
            if (gamePause.Value) {
                return;
            }
            if (context.started)
            {
                if (abilityRunner.IsAbilityReady<Boost>())
                {
                    abilityRunner.EnqueueAbilitiesInJoint<HeavyWeaponAttack, Boost>();
                }
                else {
                    abilityRunner.EnqueueAbility<HeavyWeaponAttack>();
                }
            }
            else if (context.canceled) {
                abilityRunner.Signal<HeavyWeaponAttack>();
            }
        }

        public void WeaponArt(InputAction.CallbackContext context) {
            if (gamePause.Value)
            {
                return;
            }
            if (context.started)
            {
                abilityRunner.EnqueueAbility<WeaponArt>();
            }
        }

        public void WeaponArt2(InputAction.CallbackContext context)
        {
            if (gamePause.Value)
            {
                return;
            }
            if (context.started)
            {
                abilityRunner.EnqueueAbility<LightWeaponAttack>("2");
            }
        }

        public void Guard(InputAction.CallbackContext context)
        {
            if (gamePause.Value)
            {
                return;
            }
            if (context.started)
            {
                abilityRunner.EnqueueAbility<Guard>();
            }
            else if (context.canceled)
            {
                abilityRunner.Signal<Guard>();
            }
        }

        public void Shoot(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                abilityRunner.EnqueueAbility<Shoot>();
            }
        }

        public void SwitchWeapon(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                weaponWielder.SwitchMainHand();
            }
        }

        public void PrimaryInteraction(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                interactor.Interact(InteractionType.Primary);
            }
        }

        public void SecondaryInteraction(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                interactor.Interact(InteractionType.Secondary);
            }
        }

        public void NextInteractable(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                interactor.NextInteractable();
            }
        }

        public void PreviousInteractable(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                interactor.PreviousInteractable();
            }
        }

        public void Dash(InputAction.CallbackContext context)
        {
            if (!gamePause.Value && context.started)
            {
                Ability.AbilityPipe raw = abilityRunner.GetAbilityPipe<Dash>();
                Dash.DashPipe pipe = (Dash.DashPipe)raw;
                pipe.DashDirection = move.action.ReadValue<Vector2>();
                abilityRunner.EnqueueAbility<Dash>();
            }   
        }

        public void LateUpdate()
        {
            camera.transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.z - cameraDistance);
            camera.transform.rotation = _transform.rotation;
        }
    }
}
