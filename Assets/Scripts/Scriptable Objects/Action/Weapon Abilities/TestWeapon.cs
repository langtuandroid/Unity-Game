using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem.Weapon
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(AudioBehaviour))]
    public class TestWeapon : WeaponAbility
    {
        protected override bool Action(AbilityConfig config)
        {
            throw new System.NotImplementedException();
        }

        public class TestWeaponConfig : AbilityConfig { }
    }
}
