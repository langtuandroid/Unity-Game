using LobsterFramework.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.Abilities
{
    [ComponentRequired(typeof(WeaponWielder))]
    [AddWeaponArtMenu(false, WeaponType.EmptyHand)]
    [AddAbilityMenu]
    public class Grab : Ability
    {
        protected override bool Action(AbilityConfig config, AbilityPipe pipe)
        {
            throw new System.NotImplementedException();
        }
    }
}
