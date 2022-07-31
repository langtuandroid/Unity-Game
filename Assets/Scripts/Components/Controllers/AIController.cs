using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actionable))]
public class AIController : MonoBehaviour
{
    private PhysicsUpdate physicsUpdate;
    private Actionable actionable;

    private void Start()
    {
        physicsUpdate = GetComponent<PhysicsUpdate>();
        actionable = GetComponent<Actionable>();
    }
    public void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), Space.Self);
        physicsUpdate.velocity = transform.right * 3;
        actionable.EnqueueAction<CircleAttack>();
    }
}
