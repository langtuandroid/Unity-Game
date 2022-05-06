using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Action))]
public class AIController : MonoBehaviour
{
    public void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), Space.Self);
        GetComponent<Rigidbody2D>().velocity = transform.right * 3;
        GetComponent<Action>().EnqueueAction("guard");
    }
}
