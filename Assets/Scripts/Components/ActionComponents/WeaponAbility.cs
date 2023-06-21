using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem.Weapon 
{
    [ComponentRequired(typeof(WeaponWielder))]
    public abstract class WeaponAbility : Ability
    {
        protected override sealed bool ConditionSatisfied(AbilityConfig config)
        {
            return OtherConditions(config);
        }

        protected virtual bool OtherConditions(AbilityConfig config) { return true; }
    }
}
