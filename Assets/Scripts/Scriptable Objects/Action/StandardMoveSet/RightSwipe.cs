using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility.Groups;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(CombatStat))]
    [ComponentRequired(typeof(Animator))]
    public class RightSwipe : Ability
    {
        private CombatStat combatCmp;
        private Transform transform;

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
            public Entity attacker;
            [HideInInspector]
            public float range;
            [HideInInspector]
            public int damage;
            [HideInInspector]
            public bool begin;
            [HideInInspector]
            public State state;
            [HideInInspector]
            public bool animationInterrupted;

            private ActionCapsuleCollider collider;
            private HashSet<Entity> hits;
            private List<Entity> hitQueue;
            

            public override void Initialize()
            {
                hits = new();
                hitQueue = new();
                begin = false;
            }

            public void Begin(ActionCapsuleCollider collider, Transform transform)
            {
                this.collider = collider;
                collider.transform.RotateAround(transform.position, Vector3.forward, -startAngle.Value);
                collider.SetColliderSize(new Vector2(0.5f, range));
                collider.entityEvent.AddListener((Entity entity) => { hitQueue.Add(entity); });
                collider.gameObject.SetActive(true);
                begin = true;
            }

            public void Process(Transform transform)
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
            Entity entity = abilityRunner.GetComponent<Entity>();

            foreach (RightSwipeConfig con in configs.Values)
            {
                con.range = combatCmp.attackRange.Value;
                con.attacker = entity;
                con.damage = combatCmp.attackDamage.Value;
            }
        }

        protected override void OnEnqueue(AbilityConfig config, string configName)
        {
            RightSwipeConfig con = (RightSwipeConfig)config;
            con.state = State.WindUp;
            abilityRunner.StartAnimation<RightSwipe>(configName, con.animation.Value);
        }

        protected override void SignalBody(AbilityConfig configRaw)
        {
            RightSwipeConfig config = (RightSwipeConfig)configRaw;
            switch (config.state)
            {
                case State.WindUp:
                    config.state = State.Attack;
                    config.Begin(ObjectPool.Instance.GetObject(
                        config.colliderPoolTag.Value, transform.position + transform.up * combatCmp.attackRange.Value * 0.5f,
                        transform.rotation, transform).GetComponent<ActionCapsuleCollider>(), transform);
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
                    con.Process(transform);
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
