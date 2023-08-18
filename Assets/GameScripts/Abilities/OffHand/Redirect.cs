using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    public class Redirect : Ability
    {
        [SerializeField] public Ability redirectAbility;
        [SerializeField] public string configName;

        public class RedirectConfig : AbilityConfig { }
        public class RedirectPipe : AbilityPipe { }

        protected override void Initialize()
        {

        }

        protected override void OnEnqueue(AbilityConfig config, string configName)
        {
        }

        protected override bool Action(AbilityConfig config)
        {
            return false;
        }
    }
}
