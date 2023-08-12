using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    public class Dash : Ability
    {
        public class DashConfig : AbilityConfig {
            public float dashStrength;
        }

        public class DashPipe : AbilityPipe {
            private DashConfig dashConfig;
            public override void Construct() {
                dashConfig = (DashConfig)config;
            }

            public float DashStrength { get { return dashConfig.dashStrength; } }
            public Vector2 DashDirection { get; set; } 
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            return abilityRunner.MovementController != null;
        }

        protected override bool Action(AbilityConfig config)
        {
            DashConfig c = (DashConfig)config;
            Vector2 direction = abilityRunner.TopLevelTransform.rotation * ((DashPipe)c.pipe).DashDirection;
            if (direction == Vector2.zero) {
                Vector3 back = -abilityRunner.TopLevelTransform.up;
                direction = new Vector2(back.x, back.y);
            }
            abilityRunner.MovementController.ApplyForce(direction, c.dashStrength);
            return false;
        }
    }
}
