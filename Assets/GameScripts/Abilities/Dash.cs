using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework;

namespace GameScripts.Abilities
{
    [ComponentRequired(typeof(MovementController))]
    [AddAbilityMenu]
    public class Dash : Ability
    {
        public class DashConfig : AbilityConfig
        {
            public float dashStrength;
            [Range(0, 1)] public float maxDashTime;
            [Range(0, 1)] public float moveSpeedModifier;
            [Range(0, 1)] public float rotateSpeedModifier;

            [HideInInspector] public int m_key;
            [HideInInspector] public int r_key;
        }

        public class DashPipe : AbilityPipe
        {
            private DashConfig dashConfig;
            public override void Construct()
            {
                dashConfig = (DashConfig)config;
            }

            public float DashStrength { get { return dashConfig.dashStrength; } }
            public Vector2 DashDirection { get; set; }
        }

        private MovementController controller;

        protected override void Initialize()
        {
            controller = abilityRunner.GetComponentInBoth<MovementController>();
        }

        protected override bool Action(AbilityPipe pipe)
        {
            DashConfig c = (DashConfig)CurrentConfig;
            Vector2 direction = abilityRunner.TopLevelTransform.rotation * ((DashPipe)pipe).DashDirection;
            if (direction == Vector2.zero) {
                Vector3 back = -abilityRunner.TopLevelTransform.up;
                direction = new Vector2(back.x, back.y);
            }
            controller.ApplyForce(direction, c.dashStrength);
            return false;
        }
    }
}
