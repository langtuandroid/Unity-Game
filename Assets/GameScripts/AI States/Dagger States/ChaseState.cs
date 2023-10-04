using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;
using UnityEngine.InputSystem;

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
        private Transform transform;
        private Transform targetTransform;
        private HeavyWeaponAttack.HeavyWeaponAttackPipe heavyAttackPipe;
        private float maxChargeTime;
        private float maxWalkTime;
        private float maxdirectionTime;
        private float meleeHoldTime;
        private float moveDistance;

        public override void InitializeFields(GameObject obj)
        {
            abilityRunner = controller.AbilityRunner;
            trackData = controller.GetControllerData<AITrackData>();
            transform = obj.transform;
            heavyAttackPipe = (HeavyWeaponAttack.HeavyWeaponAttackPipe)abilityRunner.GetAbilityPipe<HeavyWeaponAttack>();
        }

        public override void OnEnter()
        {
            Debug.Log("Chase");
            controller.ChaseTarget();
            targetTransform = controller.target.transform;
            maxWalkTime=Time.time;
            maxdirectionTime = Time.time;
            moveDistance = 0;
            meleeHoldTime = 0;
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

        public Type RangeAttack(float keepDistance)
        {
            if (!controller.target.gameObject.activeInHierarchy || !controller.TargetInRange(trackData.chaseDistance.Value))
            {
                controller.target = null;
                return typeof(WanderState);
            }
            if (abilityRunner.IsAbilityReady<Dash>())
            {
                Dash(false);
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
                abilityRunner.EnqueueAbility<Shoot>();
                return null;
            }
            return null;
        }
        public void Dash(bool foward)
        {
            Ability.AbilityPipe raw = abilityRunner.GetAbilityPipe<Dash>();
            Dash.DashPipe pipe = (Dash.DashPipe)raw;
            Vector3 x = targetTransform.position - transform.position;
            if (foward)
            {
               
                pipe.DashDirection = new Vector2(x.x, x.y);
            }
            else {
                pipe.DashDirection = new Vector2(-x.x, -x.y);
            }
                
                abilityRunner.EnqueueAbility<Dash>();
        }
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
            //conditionCheck(enemyHealth, enemyPosture);
            if (enemyHealth > healthThreshold)
            {
                RangeAttack(keepDistance);
            }
            else
            {
                /*float meleeDesire = meleeAttackProbability;
                float diffinHealth = healthThreshold - enemyHealth;
                meleeDesire += diffinHealth / healthThreshold * (1f- meleeDesire);*/
                float randomNumber = UnityEngine.Random.Range(0f, 1f);
                
                if (meleeHoldTime < Time.time)
                {
                    meleeHoldTime = Time.time;
                    meleeHoldTime += 0.5f;
                    if (randomNumber < meleeAttackProbability)
                    {
                        /*RangeAttack();*/
                        RangeAttack(2);
                    }
                    else
                    {
                        if (abilityRunner.IsAbilityReady<Dash>())
                        {
                            Dash(true);
                        }
                        controller.ChaseTarget();
                        MeleeAttack();
                    }
                    
                }
            }
            return null;
        }
    }
}   
