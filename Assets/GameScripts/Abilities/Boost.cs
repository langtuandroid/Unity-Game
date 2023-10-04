using LobsterFramework.AbilitySystem;
using LobsterFramework.Pool;
using LobsterFramework;
using UnityEngine;
using LobsterFramework.Utility;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(DamageModifier))]
    [ComponentRequired(typeof(Poise))]
    public class Boost : Ability
    {
        public class BoostConfig : AbilityConfig{
            public float damageModifier;
            public float duration;
            public VarString vfxTag;

            [HideInInspector] public BufferedValueAccessor<bool> hyperArmor;
            [HideInInspector] public BufferedValueAccessor<float> damageMod;
            [HideInInspector] public float timeEnd;

            protected override void Initialize()
            {
                Boost boost = (Boost)ability;
                hyperArmor = boost.poise.hyperarmor.GetAccessor();
                damageMod = boost.damageModifier.percentageDamageModifcation.GetAccessor();
            }

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
        private Transform transform;
        private Poise poise;
       
        

        protected override void Initialize()
        {
            damageModifier = abilityRunner.GetAbilityStat<DamageModifier>();
            transform = abilityRunner.TopLevelTransform;
            poise = abilityRunner.GetComponentInBoth<Poise>();
        }

        protected override void OnEnqueue(AbilityPipe pipe)
        {
            BoostConfig bConfig = (BoostConfig)CurrentConfig;
            bConfig.hyperArmor.Acquire(true);
            bConfig.timeEnd = Time.time + bConfig.duration;
            bConfig.damageMod.Acquire(bConfig.damageModifier);
            if (bConfig.vfxTag != null) {
                ObjectPool.GetObject(bConfig.vfxTag.Value, transform.position, Quaternion.identity, transform);
            }
        }

        protected override bool Action(AbilityPipe pipe)
        {
            BoostConfig bConfig = (BoostConfig)CurrentConfig;
            if (bConfig.duration == 0) 
            {
                return true;
            }
            return Time.time < bConfig.timeEnd;
        }
         
        protected override void OnActionFinish()
        {
            BoostConfig bConfig = (BoostConfig)CurrentConfig;
            bConfig.damageMod.Release();
            bConfig.hyperArmor.Release();
        }
    }
}
