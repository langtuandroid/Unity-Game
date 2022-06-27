using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Action))]
public class AIController : MonoBehaviour
{
    private PhysicsUpdate physicsUpdate;

    private void Start()
    {
        physicsUpdate = GetComponent<PhysicsUpdate>();
    }
    public void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), Space.Self);
        physicsUpdate.velocity = transform.right * 3;
        GetComponent<Action>().EnqueueAction(Setting.STD_CIRCLE_ATTACK);
    }
}
