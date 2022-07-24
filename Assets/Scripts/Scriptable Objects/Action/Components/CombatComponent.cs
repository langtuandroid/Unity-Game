using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ActionComponent(typeof(CombatComponent))]
public class CombatComponent : ActionComponent
{
    public int attackDamage;
    public float attackRange;
    public int defense;
}
