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
        new StandardMoveSet(this, o);
        o.SetAttackDamage(50);
        SetHealth(100);
        o.SetDefense(25);
        
    }

    protected override void Movement()
    {
        transform.Rotate(new Vector3(0, 0, 1), Space.Self);
        GetComponent<Rigidbody2D>().velocity = transform.right * 3;
        ((Action)GetEntityComponent(Setting.COMPONENT_ACTION)).EnqueueAction("guard");
    }
}
