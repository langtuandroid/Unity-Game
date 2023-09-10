using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(WeaponWielder))]
    public class OffhandAbility : Ability
    {
        private WeaponWielder weaponWielder;

        public class OffhandAbilityConfig : AbilityConfig { } 
        public class OffhandAbilityPipe : AbilityPipe { }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponentInBoth<WeaponWielder>();
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            if (weaponWielder.Offhand != null)
            {
                ValueTuple<Type, string> setting = weaponWielder.Offhand.AbilitySetting;
                return abilityRunner.IsAbilityReady(setting.Item1, setting.Item2);
            }
            return false;
        }

        protected override void OnEnqueue(AbilityPipe pipe)
        {
            ValueTuple<Type, string> setting = weaponWielder.Offhand.AbilitySetting;
            abilityRunner.EnqueueAbility(setting.Item1, setting.Item2);
            JoinAsSecondary(setting.Item1, setting.Item2);
        }

        protected override bool Action(AbilityPipe pipe)
        {
            // Wait until the ability finishes
            return true;
        }
    }
}
