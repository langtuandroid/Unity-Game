using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(Actionable))]
public class PlayerController : MonoBehaviour
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
    private Interactor interactor;

    public void Start(){
        player = GetComponent<Entity>();
        rb = player.GetComponent<Rigidbody2D>();
        physicsUpdate = rb.GetComponent<PhysicsUpdate>();
        actionable = GetComponent<Actionable>();
        interactor = GetComponent<Interactor>();
    }

    public void CircleAttack(InputAction.CallbackContext context)
    {
        if (!gamePause.Value && context.started) {
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

    public void Guard(InputAction.CallbackContext context) {
        if (context.started) {
            actionable.EnqueueAction<Guard>();
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        if (context.started) {
            interactor.Interact();
        }
    }

    public void LateUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
    }
}
