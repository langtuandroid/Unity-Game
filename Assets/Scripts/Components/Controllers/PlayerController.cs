using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : Interactor
{
    [SerializeField] private VoidEventChannel gameEndChannel;
    [SerializeField] private Camera camera;
    [SerializeField] private float speed;
    [SerializeField] private float distance;
    [SerializeField] private VarBool gamePause;


    private Entity player;
    private Rigidbody2D rb;
    private PhysicsUpdate physicsUpdate;
    private Actionable actionable;

    [Header("Interaction UI")]
    [SerializeField] private TMP_Text interactionPrompt1;
    [SerializeField] private TMP_Text interactionPrompt2;
    [SerializeField] private TMP_Text interactionPrompt3;
    [SerializeField] private TMP_Text interactionPrompt4;

    public void Start()
    {
        player = GetComponent<Entity>();
        rb = player.GetComponent<Rigidbody2D>();
        physicsUpdate = rb.GetComponent<PhysicsUpdate>();
        actionable = GetComponent<Actionable>();
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
        if (!context.canceled)
        {
            physicsUpdate.velocity = speed * context.ReadValue<Vector2>();
        }
        else
        {
            physicsUpdate.velocity = new Vector2(0, 0);
        }
    }

    public void Guard(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            actionable.EnqueueAction<Guard>();
        }
    }

    public void TriggerDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact(typeof(DialogueDisplayer), InteractionType.Primary);
        }
    }

    public void PrimaryInteraction(InputAction.CallbackContext context) {
        if (context.started)
        {
            Interact(InteractionType.Primary);
        }
    }

    public void SecondaryInteraction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact(InteractionType.Secondary);
        }
    }

    public void SwitchInteraction(InputAction.CallbackContext context) {
        if (context.started)
        {
            SwitchInteractable();
        }
    }

    public void LateUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
        if (interactionPrompt1 == null) {
            return;
        }
        DisplayInteractionPrompt();
    }

    private void DisplayInteractionPrompt() {
        Dictionary<InteractionType, string> options = GetInteractionOptions();

        if (options != null)
        {
            if (options.ContainsKey(InteractionType.Primary))
            {
                interactionPrompt1.text = options[InteractionType.Primary];
                interactionPrompt1.gameObject.SetActive(true);
            }
            else
            {
                interactionPrompt1.gameObject.SetActive(false);
            }

            if (options.ContainsKey(InteractionType.Secondary))
            {
                interactionPrompt2.text = options[InteractionType.Secondary];
                interactionPrompt2.gameObject.SetActive(true);
            }
            else
            {
                interactionPrompt2.gameObject.SetActive(false);
            }

            if (options.ContainsKey(InteractionType.Tertiary))
            {
                interactionPrompt3.text = options[InteractionType.Tertiary];
                interactionPrompt3.gameObject.SetActive(true);
            }
            else
            {
                interactionPrompt3.gameObject.SetActive(false);
            }

            if (options.ContainsKey(InteractionType.Quaternary))
            {
                interactionPrompt4.text = options[InteractionType.Quaternary];
                interactionPrompt4.gameObject.SetActive(true);
            }
            else
            {
                interactionPrompt4.gameObject.SetActive(false);
            }
        }
        else
        {
            interactionPrompt1.gameObject.SetActive(false);
            interactionPrompt2.gameObject.SetActive(false);
            interactionPrompt3.gameObject.SetActive(false);
            interactionPrompt4.gameObject.SetActive(false);
        }
    }
}
