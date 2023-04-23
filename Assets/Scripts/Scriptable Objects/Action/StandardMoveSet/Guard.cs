using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Pool;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.AbilitySystem
{
    [RequireAbilityStats(typeof(Guard), typeof(CombatStat))]
    [Ability(typeof(Guard))]
    public class Guard : Ability
    {
        public class GuardConfig : AbilityConfig
        {
            public RefFloat duration;
            public Sprite sprite;
            public RefFloat spriteAlpha;
            public VarString spritePoolTag;

            [HideInInspector]
            public float durationCounter;
            [HideInInspector]
            public bool actionStart;
            [HideInInspector]
            public TemporalObject guardAnimation;

            public override void Initialize()
            {
                durationCounter = 0;
                actionStart = false;
            }
        }
        private CombatStat combat;
        private Entity defender;

        protected override void Initialize()
        {
            base.Initialize();
            combat = abilityRunner.GetAbilityStat<CombatStat>();
            defender = abilityRunner.GetComponent<Entity>();
            if (defender == null)
            {
                Debug.LogError("The object is missing entity component to complete the action!");
            }
            if (combat == null)
            {
                Debug.LogError("The object is missing the required action component to complete the action!");
            }
        }

        protected override void OnEnqueue(AbilityConfig actionConfig)
        {
            GuardConfig config = (GuardConfig)actionConfig;
            config.durationCounter = 0;
            config.actionStart = true;
        }

        protected override bool ExecuteBody(AbilityConfig actionConfig)
        {
            GuardConfig config = (GuardConfig)actionConfig;
            if (!config.actionStart)
            {
                config.durationCounter += Time.deltaTime;
                if (config.durationCounter > config.duration.Value)
                {
                    return false;
                }
            }
            else
            {
                config.actionStart = false;
            }

            if (defender.IncomingDamage > 0)
            {
                Debug.Log("Blocked: " + combat.defense.Value);
            }
            defender.IncomingDamage -= combat.defense.Value;
            if (config.guardAnimation == null)
            {
                // Set up guard sprite
                GameObject g = ObjectPool.Instance.GetObject(config.spritePoolTag.Value, Vector3.zero, Quaternion.identity);
                Transform gTransform = g.transform;
                gTransform.SetParent(defender.transform);
                gTransform.localPosition = Vector3.zero;

                // Set Scale
                float range = combat.attackRange.Value;
                gTransform.localScale = Vector3.one;
                Vector3 scale = gTransform.lossyScale;
                gTransform.localScale = new Vector3(range / scale.x, range / scale.y, 1);

                TemporalObject tmp = g.GetComponent<TemporalObject>();
                tmp.ResetCounter();
                tmp.duration = -1;
                SpriteRenderer spriteRenderer = g.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = config.sprite;
                config.guardAnimation = tmp;
            }
            return true;
        }

        protected override void OnActionFinish(AbilityConfig actionConfig)
        {
            GuardConfig config = (GuardConfig)actionConfig;
            config.guardAnimation.DisableObject();
            config.guardAnimation = null;
        }
    }
}
