using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsUpdate : MonoBehaviour
{
    public Vector2 velocity;
    private Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector2();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rigidbody.velocity = velocity;
    }
}
