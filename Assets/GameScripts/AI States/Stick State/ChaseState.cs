using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;

namespace GameScripts.AI.StickEnemy
{

    [CreateAssetMenu(menuName = "StateMachine/States/AI/StickEnemy/ChaseState")]
    public class ChaseState : State
    {
        [Header("Behaviour Stats")]
        [Range(0, 1)][SerializeField] private float healthThreshold;
        [Range(0, 1)][SerializeField] private float meleeAttackProbability;
        [Range(0, 1)][SerializeField] private float walkInterval;
        [Range(0, 1)][SerializeField] private float backKeepDistance;
/*        %postureThreshold - 30
%backKeepDistance - 3
%lightAttackProb - 60%
%guardProb - 50%
%cycloneProb - 60%*/
        [SerializeField] private float attackRange;
        private AITrackData trackData;
        private AbilityRunner abilityRunner;
        private AbilityRunner playerAbilityRunner;
        private bool isNextMoveStart;
        private HeavyWeaponAttack.HeavyWeaponAttackPipe heavyAttackPipe;
        private float maxChargeTime;
        private float maxGuardChargeTime;
        private float cycloneChanceIncrease;



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
            cycloneChanceIncrease = 0;
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
                    if (!abilityRunner.IsAbilityRunning<WeaponArt>()) //if cyclone is ready
                    {
                        float cycloneChance = UnityEngine.Random.Range(0f, 1f);

                        if (cycloneChance > 0.6f - cycloneChanceIncrease) //if cyclone
                        {
                            cycloneChanceIncrease = 0;
                            abilityRunner.EnqueueAbility<WeaponArt>();
                        }
                        else
                        {
                            cycloneChanceIncrease += 0.2f;
                            float attackType = UnityEngine.Random.Range(0f, 1f);
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
                    else
                    {
                        cycloneChanceIncrease += 0.2f;
                        float attackType = UnityEngine.Random.Range(0f, 1f);
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
                return typeof(ChaseState);
            }
            return typeof(ChaseState);
        }

        public Type GuardCheck()
        {
            isNextMoveStart = false;
            if (playerAbilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || playerAbilityRunner.IsAbilityRunning<LightWeaponAttack>()|| playerAbilityRunner.IsAbilityRunning<WeaponArt>()|| playerAbilityRunner.IsAbilityRunning<Shoot>())//if player is attacking
            {
                if (!abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() && !abilityRunner.IsAbilityRunning<LightWeaponAttack>()&& !abilityRunner.IsAbilityRunning<WeaponArt>())//ai is not attacking player
                {
                    float GuardChance = UnityEngine.Random.Range(0f, 1f);
                    if (GuardChance > 0.2)
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
                    if (GuardChance > 0.4)
                    {
                        abilityRunner.EnqueueAbility<Guard>();
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
            if (abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || abilityRunner.IsAbilityRunning<LightWeaponAttack>()|| abilityRunner.IsAbilityRunning<Guard>()|| abilityRunner.IsAbilityRunning<WeaponArt>()) 
            {
                if(abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() && maxChargeTime<Time.time )
                {
                    abilityRunner.Signal<HeavyWeaponAttack>();
                }
                return typeof(ChaseState);
            }
            MeleeAttack();
            return typeof(ChaseState);
        }
    }
}   
