using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LobsterFramework.AbilitySystem
{
    [ComponentRequired(typeof(WeaponWielder))]
    [AddAbilityMenu]
    public class WeaponArt : Ability
    {
        private WeaponWielder weaponWielder;

        public class WeaponArtConfig : AbilityConfig { }
        public class WeaponArtPipe : AbilityPipe { }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponentInBoth<WeaponWielder>();
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            if (weaponWielder.Mainhand != null) {
                ValueTuple<Type, string> setting = weaponWielder.Mainhand.AbilitySetting;
                return abilityRunner.IsAbilityReady(setting.Item1, setting.Item2);
            }
            return false;
        }

        protected override void OnEnqueue(AbilityConfig config, AbilityPipe pipe, string configName)
        {
            ValueTuple<Type, string> setting = weaponWielder.Mainhand.AbilitySetting;
            abilityRunner.EnqueueAbility(setting.Item1, setting.Item2);
        }

        protected override bool Action(AbilityConfig config, AbilityPipe pipe)
        {
            return false;
        }
    }
}
