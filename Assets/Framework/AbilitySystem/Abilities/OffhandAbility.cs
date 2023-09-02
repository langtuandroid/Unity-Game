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

        public class RedirectConfig : AbilityConfig { }
        public class RedirectPipe : AbilityPipe { }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            return weaponWielder.Offhand != null;
        }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponentInBoth<WeaponWielder>();
        }

        protected override void OnEnqueue(AbilityConfig config, AbilityPipe pipe)
        {
            ValueTuple<Type, string> pair = weaponWielder.Offhand.AbilitySetting;
            abilityRunner.EnqueueAbility(pair.Item1, pair.Item2);
        }

        protected override bool Action(AbilityConfig config, AbilityPipe pipe)
        {
            return false;
        }
    }
}
