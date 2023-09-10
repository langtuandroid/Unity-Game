using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    public abstract class WeaponAbility : AbilityCoroutine
    {
        protected WeaponWielder WeaponWielder { get; private set; }
        protected bool IsMainhanded { get;private set; }

        protected sealed override void Initialize()
        {
            WeaponWielder = abilityRunner.GetComponentInBoth<WeaponWielder>();
            IsMainhanded = !OffhandWeaponAbilityAttribute.IsOffhand(GetType());
            Init();
        }

        protected virtual void Init() { }

        protected sealed override bool ConditionSatisfied(AbilityConfig config)
        {
            Weapon query;
            if (IsMainhanded) {
                query = WeaponWielder.Mainhand;
            }
            else {
                query = WeaponWielder.Offhand;
            }
            return query != null && RunningCount == 0 && RequireWeaponStatAttribute.HasWeaponStats(GetType(), WeaponWielder) && WConditionSatisfied(config);
        }

        protected virtual bool WConditionSatisfied(AbilityConfig config) { return true; }
    }
}
