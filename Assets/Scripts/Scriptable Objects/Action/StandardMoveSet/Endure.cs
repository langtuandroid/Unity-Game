using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// A supplimental ability that provides hyperarmor for the entity that casts it
    /// </summary>
    [Ability(typeof(Endure))]
    public class Endure : Ability
    {
        public class EndureConfig : AbilityConfig
        {
            public bool useTimer;
            public double duration;
            public double startTime;
            [HideInInspector]
            public int effector_id;
        }

        protected override void OnEnqueue(AbilityConfig config)
        {
            EndureConfig endureConfig = (EndureConfig)config;
            if (endureConfig.useTimer)
            {
                endureConfig.startTime = Time.timeAsDouble;
            }
            endureConfig.effector_id = abilityRunner.HyperArmor();
        }

        protected override bool ExecuteBody(AbilityConfig config)
        {
            EndureConfig endureConfig = (EndureConfig)config;
            if (endureConfig.useTimer) {
                if (Time.timeAsDouble - endureConfig.startTime >= endureConfig.duration) {
                    return false;
                }
            }
            return true;
        }

        protected override void OnActionFinish(AbilityConfig config)
        {
            EndureConfig endureConfig = (EndureConfig)config;
            abilityRunner.DisArmor(endureConfig.effector_id);
            Debug.Log("DisArmor");
        }
    }
}
