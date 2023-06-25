using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private RefFloat speed;

    private Rigidbody2D rigidbody2d;
    private Transform localTransform;

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        localTransform = GetComponent<Transform>();
    }

    public void Activate() { 
        rigidbody2d.velocity = Vector3.Normalize(localTransform.up) * speed.Value;
    }
}
