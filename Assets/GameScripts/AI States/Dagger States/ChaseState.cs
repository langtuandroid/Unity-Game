using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;

namespace GameScripts.AI.DaggerEnemy
{

    [CreateAssetMenu(menuName = "StateMachine/States/AI/DaggerEnemy/ChaseState")]
    public class ChaseState : State
    {
        [SerializeField] private float attackRange;
        [Header("Behaviour Stats")]
        [Range(0,1)][SerializeField] private float healthThreshold;
        [Range(0, 1)][SerializeField] private float meleeAttackProbability;
        [Range(0, 1)][SerializeField] private float walkInterval;
        [SerializeField] private float keepDistance;
        private AITrackData trackData;
        private AbilityRunner abilityRunner;
        float meleeOn = 0;
        private Transform transform;
        private Mana manaComponent;
        private Entity chaseTarget;
        private Transform targetTransform;
        private HeavyWeaponAttack.HeavyWeaponAttackPipe heavyAttackPipe;
        private float maxChargeTime;
        private float maxWalkTime;
        private float maxdirectionTime;
        private float moveDistance;

        public override void InitializeFields(GameObject obj)
        {
            abilityRunner = controller.AbilityRunner;
            trackData = controller.GetControllerData<AITrackData>();
            manaComponent = abilityRunner.GetAbilityStat<Mana>();
            transform = obj.transform;
            heavyAttackPipe = (HeavyWeaponAttack.HeavyWeaponAttackPipe)abilityRunner.GetAbilityPipe<HeavyWeaponAttack>();
        }

        public override void OnEnter()
        {
            Debug.Log("Chase");
            controller.ChaseTarget();
            chaseTarget = controller.target;
            targetTransform = controller.target.transform;
            meleeOn = 0;
            maxWalkTime=Time.time;
            maxdirectionTime = Time.time;
            moveDistance = 0;
        }

        public override void OnExit()
        {
            
        }
        public Type MeleeAttack()
        {
            if (!controller.target.gameObject.activeInHierarchy)
            {
                return typeof(WanderState);
            }
            if (controller.TargetInRange(trackData.chaseDistance.Value))
            {
                if (controller.TargetInRange(attackRange))
                {
                    controller.stopChaseTarget();
                    float randomNumber = UnityEngine.Random.Range(0f, 1f);
                    Debug.Log(randomNumber);
                    if (randomNumber > 0.5)
                    {
                        abilityRunner.EnqueueAbility<LightWeaponAttack>();
                    }
                    else
                    {
                        abilityRunner.EnqueueAbility<HeavyWeaponAttack>();
                        float randomChargeTime = UnityEngine.Random.Range(0.5f, 1.5f);
                        maxChargeTime = Time.time;
                        maxChargeTime += randomChargeTime * heavyAttackPipe.MaxChargeTime;

                    }
                }
                return typeof(ChaseState);
            }
            return typeof(WanderState);
        }

        public Type RangeAttack()
        {
            if (!controller.target.gameObject.activeInHierarchy || !controller.TargetInRange(trackData.chaseDistance.Value))
            {
                controller.target = null;
                return typeof(WanderState);
            }
            if (maxWalkTime < Time.time)
            {
                if(maxdirectionTime < Time.time)
                {
                    moveDistance = UnityEngine.Random.Range(-1f, 1f);
                    maxdirectionTime = Time.time + UnityEngine.Random.Range(0.5f, 1f);
                }
                controller.KeepDistanceFromTarget(transform.position, keepDistance, moveDistance);
                maxWalkTime = Time.time + walkInterval;
            }
            if (controller.TargetVisible(controller.transform.position, trackData.engageDistance.Value)) //if in visible area
            {
                
                /*if (controller.TargetInRange(trackData.keepDistance.Value))
                {
                    controller.MoveInDirection(transform.position - targetTransform.position, trackData.keepDistance.Value - Vector3.Distance(transform.position, targetTransform.position));
                }*/
                abilityRunner.EnqueueAbility<Shoot>();
                return null;
            }
            /*controller.ChaseTarget();*/
            return null;
        }
/*        public Type conditionCheck(float health, float posture)
        {
            if(posture<0.4 && health>0.5)
            {

            }
            return null;
        }*/
        public override Type Tick()
        {
            if (!controller.target.gameObject.activeInHierarchy || !controller.TargetInRange(trackData.chaseDistance.Value))
            {
                controller.ResetTarget();
                return typeof(WanderState);
            }
            controller.LookTowards();
            if (!abilityRunner.IsAbilityReady<HeavyWeaponAttack>()|| !abilityRunner.IsAbilityReady<LightWeaponAttack>())
            {
                if(!abilityRunner.IsAbilityReady<HeavyWeaponAttack>() && maxChargeTime< Time.time )
                {
                    abilityRunner.Signal<HeavyWeaponAttack>();
                }
                return null;
            }
            
            float enemyHealth = controller.target.Health / controller.target.MaxHealth;
            float enemyPosture = controller.target.Posture / controller.target.MaxPosture;
            //conditionCheck(enemyHealth, enemyPosture);
            if (enemyHealth > healthThreshold && meleeOn!=1)
            {
                RangeAttack();
            }
            else
            {
                
                float meleeDesire = meleeAttackProbability;
                float diffinHealth = healthThreshold - enemyHealth;
                meleeDesire += diffinHealth / healthThreshold * (1f- meleeDesire);
                float randomNumber = UnityEngine.Random.Range(0f, 1f);
                if (randomNumber > meleeDesire && meleeOn != 1)
                {
                    RangeAttack();
                }
                meleeOn = 1;
                controller.ChaseTarget();
                MeleeAttack();
            }
            return null;
        }
    }
}   
