using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    public abstract class WeaponAbility : AbilityCoroutine
    {
        protected WeaponWielder WeaponWielder { get; private set; }

        protected sealed override void Initialize()
        {
            WeaponWielder = abilityRunner.GetComponentInBoth<WeaponWielder>();
            Init();
        }

        protected virtual void Init() { }

        protected sealed override bool ConditionSatisfied(AbilityConfig config)
        {
            return RequireWeaponStatAttribute.HasWeaponStats(GetType(), WeaponWielder) && WConditionSatisfied(config);
        }

        protected virtual bool WConditionSatisfied(AbilityConfig config) { return true; }
    }
}
