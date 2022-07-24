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

    [SerializeField] private RefBool t1;
    [SerializeField] private RefBool t2;

    //[SerializeField] private RefActionData testData;
   // [SerializeField] private ActionData data;
    //[SerializeField] private ActionInstance testAttack;

    private Entity player;
    private Rigidbody2D rb;
    private PhysicsUpdate physicsUpdate;
    private Actionable actionable;

    public void Start(){
        player = GetComponent<Entity>();
        rb = player.GetComponent<Rigidbody2D>();
        physicsUpdate = rb.GetComponent<PhysicsUpdate>();
        actionable = GetComponent<Actionable>();
        actionable.AddActionComponent<CombatComponent>();
        // actionable.AddActionInstance<CircleAttack>();
        // actionable.AddActionInstance<Guard>();
        // testAttack = ScriptableObject.CreateInstance<CircleAttack>();
    }

    public void CircleAttack()
    {
        if (!gamePause.Value) {
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
        actionable.EnqueueAction<Guard>();
    }

    public void LateUpdate()
    {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
    }
}
