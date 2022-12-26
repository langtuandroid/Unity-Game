using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Animator))]
public class AIAnimationConfigurator : MonoBehaviour
{
    private AIPath ai;
    private Animator animator;
    void Start()
    {
        ai = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isMoving", !ai.reachedDestination);
    }
}
