using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using LobsterFramework.AbilitySystem;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(CombatStat))]
    public class RightSwipe : Ability
    {
        private CombatStat combatCmp;
        private Transform transform;
        private Entity entity;

        public enum State
        {
            WindUp,
            Attack,
            End
        }

        public class RightSwipeConfig : AbilityConfig
        {
            public VarString colliderPoolTag;
            public RefFloat startAngle;
            public RefFloat rotateSpeed;
            public VarString animation;
            public TargetSetting targetSetting;
            public List<Effect> effects;

            [HideInInspector]
            public bool begin;
            [HideInInspector]
            public State state;
            [HideInInspector]
            public bool animationInterrupted;

            private ActionCapsuleCollider collider;
            private HashSet<Entity> hits;
            private List<Entity> hitQueue;
            

            protected override void Initialize()
            {
                hits = new();
                hitQueue = new();
                begin = false;
            }

            public void Begin(ActionCapsuleCollider collider, Transform transform, float range)
            {
                this.collider = collider;
                collider.transform.RotateAround(transform.position, Vector3.forward, -startAngle.Value);
                collider.SetColliderSize(new Vector2(0.5f, range));
                collider.entityEvent.AddListener((Entity entity) => { hitQueue.Add(entity); });
                collider.gameObject.SetActive(true);
                begin = true;
            }

            public void Process(Transform transform, Entity attacker, float damage)
            {
                int id = transform.gameObject.GetInstanceID();
                foreach (Entity entity in hitQueue)
                {
                    if (entity.gameObject.GetInstanceID() != id && !hits.Contains(entity) && targetSetting.IsTarget(entity))
                    {
                        hits.Add(entity);
                        entity.Damage(damage, 0, attacker);
                    }
                }
                hitQueue.Clear();
                collider.transform.RotateAround(transform.position, Vector3.forward, Time.deltaTime * rotateSpeed.Value);
            }

            public void Finish()
            {
                collider.gameObject.SetActive(false);
                hitQueue.Clear();
                hits.Clear();
            }
        }

        protected override void Initialize()
        {
            combatCmp = abilityRunner.GetAbilityStat<CombatStat>();
            transform = abilityRunner.GetComponent<Transform>();
            entity = abilityRunner.GetComponent<Entity>();
        }

        protected override void OnEnqueue(AbilityConfig config, string configName)
        {
            RightSwipeConfig con = (RightSwipeConfig)config;
            con.state = State.WindUp;
            abilityRunner.StartAnimation<RightSwipe>(configName, con.animation.Value);
        }

        protected override void Signal(AbilityConfig configRaw, bool isAnimation)
        {
            RightSwipeConfig config = (RightSwipeConfig)configRaw;
            switch (config.state)
            {
                case State.WindUp:
                    config.state = State.Attack;
                    config.Begin(ObjectPool.Instance.GetObject(
                        config.colliderPoolTag.Value, transform.position + transform.up * combatCmp.attackRange.Value * 0.5f,
                        transform.rotation, transform).GetComponent<ActionCapsuleCollider>(), transform, combatCmp.attackRange);
                    break;
                case State.Attack:
                    config.state = State.End;
                    break;
                default:
                    return;
            }
        }


        protected override bool Action(AbilityConfig config)
        {
            RightSwipeConfig con = (RightSwipeConfig)config;
            if (con.animationInterrupted) {
                con.animationInterrupted = false;
                return false;
            }
            switch (con.state)
            {
                case State.WindUp:
                    return true;
                case State.Attack:
                    con.Process(transform, entity, combatCmp.attackDamage.Value);
                    return true;
                default:
                    return false;
            }
        }

        protected override void OnActionFinish(AbilityConfig config)
        {
            RightSwipeConfig con = (RightSwipeConfig)config;
            con.Finish();
        }
    }
}
