using UnityEngine;
using UnityEngine.InputSystem;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Interaction;
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
        [SerializeField] private Vector2 cameraDeviation;

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
        [SerializeField] private InputActionAsset inputAsset;
        private InputActionMap gameplayInputs;
        private InputActionMap pauseMenuInputs;
        private InputActionMap inventoryInputs;
        [SerializeField] private InputActionReference rotate;
        [SerializeField] private InputActionReference move;
        [SerializeField] private InputActionReference mouse;
        [SerializeField] private float rotateAcceleration;
        [SerializeField] private float mouseSensitivity;
        private float rotateVelocity;

        [Header("UI")]
        [SerializeField] private GameObject gameplayUI;
        [SerializeField] private GameObject respawnUI;
        [SerializeField] private GameObject inventoryUI;
        [SerializeField] private GameObject pauseUI;

        private Transform _transform;

        public void Start()
        {
            playerRespawnChennel.OnEventRaised += RespawnPlayer;
            _transform = GetComponent<Transform>(); 
            moveControl = GetComponent<MovementController>();
            player = GetComponent<Entity>();

            SetupInput();
            if (GameManager.Instance.UseAlternativeInput)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void SetupInput()
        {
            inputAsset.actionMaps[0].Enable();
            gameplayInputs = inputAsset.actionMaps[1];
            inventoryInputs = inputAsset.actionMaps[2];
            pauseMenuInputs = inputAsset.actionMaps[3];

            DisableInputs();
            gameplayInputs.Enable();
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.UseAlternativeInput)
            {
                moveControl.RotateTowards(camera.ScreenToWorldPoint(mouse.action.ReadValue<Vector2>()) - transform.position);
            }
            else {
                Vector2 input = rotate.action.ReadValue<Vector2>();
                if (input.x != 0)
                {
                    float delta = input.x * Time.deltaTime * mouseSensitivity;
                    rotateVelocity = Mathf.MoveTowards(rotateVelocity, -delta, rotateAcceleration * Time.deltaTime);
                    rotateVelocity = Mathf.Clamp(rotateVelocity, -moveControl.RotateSpeed, moveControl.RotateSpeed);
                }
                else
                {
                    rotateVelocity = Mathf.MoveTowards(rotateVelocity, 0, rotateAcceleration * Time.deltaTime);
                }
                moveControl.RotateByDegrees(rotateVelocity);
            }
        }

        private void Update()
        {
            Vector2 direction;
            if (GameManager.Instance.UseAlternativeInput)
            {
                direction = Quaternion.Inverse(transform.rotation) * move.action.ReadValue<Vector2>();
            }
            else {
                direction = move.action.ReadValue<Vector2>();
            }
            moveControl.Move(direction);
        }

        private void OnDisable()
        {
            if (player.IsDead)
            {
                OnPlayerDeath();
            }
        }

        private void OnDestroy()
        {
            playerRespawnChennel.OnEventRaised -= RespawnPlayer;
        }

        public void RespawnPlayer()
        {
            gameplayUI.SetActive(true);
            respawnUI.SetActive(false);
            gameplayInputs.Enable();
            if (GameManager.Instance.UseAlternativeInput)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            player.ResetStatus();
        }
        private void OnPlayerDeath()
        {
            Debug.Log("Waiting for Respawn!");
            gameplayUI.SetActive(false);
            inventoryUI.SetActive(false);
            respawnUI.SetActive(true);
            DisableInputs();
            Cursor.lockState = CursorLockMode.None;
        }
         
        private void DisableInputs() {
            for (int i = 1;i < inputAsset.actionMaps.Count;i++) {
                InputActionMap map = inputAsset.actionMaps[i];
                map.Disable();
            }
        }

        public void WeaponLightAttack(InputAction.CallbackContext context)
        {
            if (context.started)
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
            if (context.started)
            {
                abilityRunner.EnqueueAbility<WeaponArt>();
            }
        }

        public void WeaponArt2(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                abilityRunner.EnqueueAbility<LightWeaponAttack>("2");
            }
        }

        public void OffhandAbility(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                abilityRunner.EnqueueAbility<OffhandAbility>();
            }
        }

        public void SwitchWeapon(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                weaponWielder.SwitchMainHand();
            }
        }

        public void SwitchOffhandWeapon(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                weaponWielder.SwitchOffHand();
            }
        }

        public void PrimaryInteraction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                interactor.Interact(InteractionType.Primary);
            }
        }

        public void SecondaryInteraction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                interactor.Interact(InteractionType.Secondary);
            }
        }

        public void NextInteractable(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                interactor.NextInteractable();
            }
        }

        public void PreviousInteractable(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                interactor.PreviousInteractable();
            }
        }

        public void Dash(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Ability.AbilityPipe raw = abilityRunner.GetAbilityPipe<Dash>();
                Dash.DashPipe pipe = (Dash.DashPipe)raw;
                if (GameManager.Instance.UseAlternativeInput)
                {
                    pipe.DashDirection = move.action.ReadValue<Vector2>();
                }
                else {
                    pipe.DashDirection = abilityRunner.TopLevelTransform.rotation * move.action.ReadValue<Vector2>();
                }
                
                abilityRunner.EnqueueAbility<Dash>();
            }   
        }

        public void Guard(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                abilityRunner.EnqueueAbility<Guard>();
            }
            else if (context.canceled)
            {
                abilityRunner.Signal<Guard>();
            }
        }

        public void InventoryOpen(InputAction.CallbackContext context) {
            if (context.started) {
                gameplayInputs.Disable();
                inventoryInputs.Enable();
                Cursor.lockState = CursorLockMode.None;
                gameplayUI.SetActive(false);
                inventoryUI.SetActive(true);
            }
        }

        public void InventoryClose(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                gameplayInputs.Enable();
                inventoryInputs.Disable();
                gameplayUI.SetActive(true);
                inventoryUI.SetActive(false);
                if (GameManager.Instance.UseAlternativeInput)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        public void PauseGame(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("Game Pause!");
                GameManager.GamePaused = true;
                gameplayInputs.Disable();
                pauseMenuInputs.Enable();
                pauseUI.SetActive(true);
                gameplayUI.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
        }

        public void ResumeGame(InputAction.CallbackContext context) {
            if (context.started)
            {
                Debug.Log("Game Resume!");
                GameManager.GamePaused = false;
                gameplayInputs.Enable();
                pauseMenuInputs.Disable();
                gameplayUI.SetActive(true);
                pauseUI.SetActive(false);
                if (GameManager.Instance.UseAlternativeInput)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                
                Time.timeScale = 1;
            }
        }

        public void LateUpdate()
        {
            camera.transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.z - cameraDistance);
            if (!GameManager.Instance.UseAlternativeInput)
            {
                camera.transform.rotation = _transform.rotation;
                Vector3 deviation = new Vector3(cameraDeviation.x, cameraDeviation.y, 0);
                deviation = camera.transform.rotation * deviation;
                camera.transform.position += deviation;
            }
            else {
                camera.transform.rotation = Quaternion.identity;
            }
        }
    }
}
