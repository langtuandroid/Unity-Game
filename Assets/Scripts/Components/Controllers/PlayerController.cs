using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(GeneralInteractor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float distance;
    [SerializeField] private VarBool gamePause;

    [Header("Event Channels")]
    [SerializeField] private VoidEventChannel gameEndChannel;
    [SerializeField] private VoidEventChannel playerDeathChannel;
    [SerializeField] private VoidEventChannel playerRespawnChennel;

    private Entity player;
    private Rigidbody2D rb;
    private Actionable actionable;
    private GeneralInteractor interactor;

    private float angle;
    private Vector2 steering;

    public void Start()
    {
        player = GetComponent<Entity>();
        actionable = GetComponent<Actionable>();
        interactor = GetComponent<GeneralInteractor>();
        rb = GetComponent<Rigidbody2D>();

        playerRespawnChennel.OnEventRaised += RespawnPlayer;
    }

    private void FixedUpdate()
    {
        if (steering != Vector2.zero) {
            rb.AddForce(steering);
        }
       
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed);
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

    private void RespawnPlayer() {
        player.Reset();
        actionable.Reset();
    }

    public void CircleAttack(InputAction.CallbackContext context)
    {
        if (!gamePause.Value && context.started)
        {
            actionable.EnqueueAction<CircleAttack>();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (gamePause.Value) {
            return;
        }
        if (!context.canceled)
        {
            steering = (acceleration * context.ReadValue<Vector2>().normalized);
        }
        else
        {
            steering = Vector2.zero;
        }
    }

    public void Guard(InputAction.CallbackContext context)
    {
        if (!gamePause.Value && context.started)
        {
            actionable.EnqueueAction<Guard>();
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (!gamePause.Value && context.started)
        {
            actionable.EnqueueAction<Shoot>();
        }
    }

    public void TriggerDialogue(InputAction.CallbackContext context)
    {
        if (!gamePause.Value && context.started)
        {
            interactor.Interact(typeof(DialogueDisplayer), InteractionType.Primary);
        }
    }

    public void PrimaryInteraction(InputAction.CallbackContext context) {
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

    public void NextInteractable(InputAction.CallbackContext context) {
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

    public void LookAtMouse(InputAction.CallbackContext context) {
        if (gamePause.Value || Camera.main == null) {
            return;
        }
        // If the mouse hasn't moved, do not update the destination angle
        Vector2 mouse_pos = context.ReadValue<Vector2>();
        Vector2 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 current = new Vector2(camera.transform.up.x, camera.transform.up.y);
        Vector2 dir = new Vector2(mouse_pos.x - object_pos.x, mouse_pos.y - object_pos.y);
        angle = Vector2.SignedAngle(current, dir);

        float currentAngle = transform.rotation.eulerAngles.z;
        float angleDif = Mathf.DeltaAngle(angle, currentAngle);
        if (Mathf.Abs(angleDif) > 0)
        {
            float rotateMax = rotateSpeed * Time.deltaTime;
            if (Mathf.Abs(angleDif) > rotateMax && rotateSpeed > 0)
            {
                if (angleDif > 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle - rotateMax));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle + rotateMax));
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }
    }

    public void LateUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
    }
}
