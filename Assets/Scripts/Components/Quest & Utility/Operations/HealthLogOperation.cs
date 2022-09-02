using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthLogOperation : Operation
{
    [SerializeField] private Entity entity;

    public override void Begin()
    {
        Debug.Log("Selected Entity Health: " + entity.Health);
    }
}
