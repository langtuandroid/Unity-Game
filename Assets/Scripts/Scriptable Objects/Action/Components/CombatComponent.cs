using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Action
{
    [ActionComponent(typeof(CombatComponent))]
    public class CombatComponent : ActionComponent
    {
        public RefInt attackDamage;
        public RefFloat attackRange;
        public RefInt defense;
    }
}
