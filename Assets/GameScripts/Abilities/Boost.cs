using LobsterFramework.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(DamageModifier))]
    public class Boost : Ability
    {
        public class BoostConfig : AbilityConfig{
            public float damageModifier;
            public float duration;

            [HideInInspector] public int damageKey;
            [HideInInspector] public int hyperArmorKey;
            [HideInInspector] public float timeEnd;
            protected override void Validate()
            {
                if (damageModifier < 0)
                {
                    damageModifier = 0;
                }
                if (duration < 0) {
                    duration = 0;
                }
            }
        }

        public class BoostPipe : AbilityPipe {
            private BoostConfig bConfig;

            public override void Construct()
            {
                bConfig = (BoostConfig)config;
            }

            public float Duration
            {
                get { return bConfig.duration; }
            }
        }

        private DamageModifier damageModifier;

        protected override void Initialize()
        {
            damageModifier = abilityRunner.GetAbilityStat<DamageModifier>();
        }

        protected override void OnEnqueue(AbilityConfig config, AbilityPipe pipe, string configName)
        {
            BoostConfig bConfig = (BoostConfig)config;
            bConfig.hyperArmorKey = abilityRunner.HyperArmor();
            bConfig.timeEnd = Time.time + bConfig.duration;
            bConfig.damageKey = damageModifier.percentageDamageModifcation.AddEffector(bConfig.damageModifier);
        }

        protected override bool Action(AbilityConfig config, AbilityPipe pipe)
        {
            BoostConfig bConfig = (BoostConfig)config;
            if (bConfig.duration == 0) 
            {
                return true;
            }
            return Time.time < bConfig.timeEnd;
        }

        protected override void OnActionFinish(AbilityConfig config)
        {
            BoostConfig bConfig = (BoostConfig)config;
            abilityRunner.DisArmor(bConfig.hyperArmorKey);
            damageModifier.percentageDamageModifcation.RemoveEffector(bConfig.damageKey);
        }
    }
}
