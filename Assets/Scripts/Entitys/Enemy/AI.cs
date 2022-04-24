using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Entity
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Combat o = new Combat(this);
        SetEntityComponent("Combat", o);
        o.SetAttackDamage(50);
        base.SetHealth(100);
        o.SetDefense(25);
        Dictionary<string, object> args = new Dictionary<string, object> { 
            ["defender"] = this
        };
        // ((Action)GetEntityComponent("Action")).EnqueueAction("guard", args);
    }

    protected override void Movement()
    {
        transform.Rotate(new Vector3(0, 0, 1), Space.Self);
        GetComponent<Rigidbody2D>().velocity = transform.right * 3;
        // ((Action)GetEntityComponent("Action")).EnqueueAction("guard");
    }
}
