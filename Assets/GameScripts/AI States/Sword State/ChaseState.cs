using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;

namespace GameScripts.AI.SwordEnemy
{

    [CreateAssetMenu(menuName = "StateMachine/States/AI/SwordEnemy/ChaseState")]
    public class ChaseState : State
    {
        [SerializeField] private float attackRange;
        private AITrackData trackData;
        private AbilityRunner abilityRunner;
        private AbilityRunner playerAbilityRunner;
        private bool isNextMoveStart;
        private HeavyWeaponAttack.HeavyWeaponAttackPipe heavyAttackPipe;
        private float maxChargeTime;
        private float maxGuardChargeTime;
        private float endureChanceIncrease;



        public override void InitializeFields(GameObject obj)
        {
            abilityRunner = controller.AbilityRunner;
            playerAbilityRunner = controller.PlayerAbilityRunner;
            trackData = controller.GetControllerData<AITrackData>();
            heavyAttackPipe = (HeavyWeaponAttack.HeavyWeaponAttackPipe)abilityRunner.GetAbilityPipe<HeavyWeaponAttack>();
        }

        public override void OnEnter()
        {
            controller.ChaseTarget();
            isNextMoveStart = false;
            endureChanceIncrease = 0;
            playerAbilityRunner.onAbilityEnqueued += OnPlayerAction;

        }

        public override void OnExit()
        {
            playerAbilityRunner.onAbilityEnqueued -= OnPlayerAction;
        }
        private void OnPlayerAction(Type abilityType)
        {
            isNextMoveStart = true;
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
                    float attackType = 0.3f; 
                    float endureChance = UnityEngine.Random.Range(0f, 1f);
                    if (endureChance > 0.7f - endureChanceIncrease) //if boost
                    {
                        endureChanceIncrease = 0;
                        if (attackType > 0.4)
                        {
                            if(!abilityRunner.EnqueueAbilitiesInJoint<LightWeaponAttack, Boost>())
                            {
                                abilityRunner.EnqueueAbility<LightWeaponAttack>();
                            }
                        }
                        else
                        {
                            float randomChargeTime = UnityEngine.Random.Range(0.5f, 1.5f);
                            if (abilityRunner.EnqueueAbilitiesInJoint<HeavyWeaponAttack, Boost>())
                            {
                                maxChargeTime = Time.time;
                                maxChargeTime += randomChargeTime * heavyAttackPipe.MaxChargeTime;
                            }
                            else
                            {
                                abilityRunner.EnqueueAbility<HeavyWeaponAttack>();
                                maxChargeTime = Time.time;
                                maxChargeTime += randomChargeTime * heavyAttackPipe.MaxChargeTime;
                            }
                        }
                    }
                    else
                    {
                        endureChanceIncrease += 0.2f;
                        if (attackType > 0.4)
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

                }
                return null;
            }
            return null;
        }

        public Type GuardCheck()
        {
            isNextMoveStart = false;
            if (playerAbilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || playerAbilityRunner.IsAbilityRunning<LightWeaponAttack>())//if player is attacking
            {
                if (!abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() && !abilityRunner.IsAbilityRunning<LightWeaponAttack>())//ai is not attacking
                {
                    float GuardChance = UnityEngine.Random.Range(0f, 1f);
                    if (GuardChance > 0.5)
                    {
                        abilityRunner.EnqueueAbility<Guard>();
                        float randomGuardChargeTime = UnityEngine.Random.Range(0.5f, 1f);
                        maxGuardChargeTime = Time.time;
                        maxGuardChargeTime += randomGuardChargeTime;
                    }

                }
                else
                {
                    float GuardChance = UnityEngine.Random.Range(0f, 1f);
                    if (GuardChance > 0.2)
                    {
                        abilityRunner.EnqueueAbility<Guard>();
                        abilityRunner.Signal<HeavyWeaponAttack>();
                        abilityRunner.Signal<LightWeaponAttack>();
                        float randomGuardChargeTime = UnityEngine.Random.Range(0.5f, 1f);
                        maxGuardChargeTime = Time.time;
                        maxGuardChargeTime += randomGuardChargeTime;
                    }
                }
            }
            return null;
        }
        public override Type Tick()
        {
            if (!controller.target.gameObject.activeInHierarchy || !controller.TargetInRange(trackData.chaseDistance.Value))
            {
                controller.ResetTarget();
                return typeof(WanderState);
            }
            controller.LookTowards();
            if (abilityRunner.IsAbilityRunning<Guard>() && maxGuardChargeTime < Time.time)
            {
                abilityRunner.Signal<Guard>();
            }
            if (controller.TargetInRange(attackRange) && isNextMoveStart==true)
            {
                GuardCheck();
            }
            if (abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || abilityRunner.IsAbilityRunning<LightWeaponAttack>()|| abilityRunner.IsAbilityRunning<Guard>())
            {
                if(abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() && maxChargeTime<Time.time )
                {
                    abilityRunner.Signal<HeavyWeaponAttack>();
                }
                return null;
            }
            MeleeAttack();
            return null;
        }
    }
}   
