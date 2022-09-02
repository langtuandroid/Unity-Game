using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCheckCondition : Condition
{
    [SerializeField] private Entity entity;
    [SerializeField] private int health;

    public override bool Eval()
    {
        return entity.Health < health;
    }
}
