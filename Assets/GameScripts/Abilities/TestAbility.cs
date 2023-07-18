using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;

namespace GameScripts.Abilities
{
    [RequireAbilityStats(typeof(Mana))]
    public abstract class TestAbility : Ability
    {
    }
}
