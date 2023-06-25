using UnityEngine;
using LobsterFramework.AbilitySystem.Weapon;
using LobsterFramework.AbilitySystem;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(Animator))]
    public class HeavyWeaponAttack : WeaponAbility
    {
        public class HeavyWeaponAttackConfig : AbilityConfig { }

        protected override bool Action(AbilityConfig config)
        {
            throw new System.NotImplementedException();
        }
    }
}
