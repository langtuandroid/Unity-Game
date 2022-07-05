using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actionable))]
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

    public void Start(){
        player = GetComponent<Entity>();
        rb = player.GetComponent<Rigidbody2D>();
        physicsUpdate = rb.GetComponent<PhysicsUpdate>();
    }

    public void CircleAttack()
    {
        if (!gamePause.value) {
            GetComponent<Actionable>().EnqueueAction(Setting.STD_CIRCLE_ATTACK);
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
        GetComponent<Actionable>().EnqueueAction(Setting.STD_GUARD);
    }

    public void LateUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
    }
}
