using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework
{
    [Ability(typeof(Combo))]
    public class Combo : Ability
    {
        

        protected override bool ExecuteBody(AbilityConfig config)
        {
            throw new System.NotImplementedException();
        }

        public class ComboConfig : AbilityConfig { }
    }
}
