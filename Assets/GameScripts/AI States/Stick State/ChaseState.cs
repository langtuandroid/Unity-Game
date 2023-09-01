using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;
using static UnityEngine.EventSystems.EventTrigger;

namespace GameScripts.AI.StickEnemy
{

    [CreateAssetMenu(menuName = "StateMachine/States/AI/StickEnemy/ChaseState")]
    public class ChaseState : State
    {
        [SerializeField] private float attackRange;
        private AITrackData trackData;
        private AbilityRunner abilityRunner;
        private AbilityRunner playerAbilityRunner;
        private bool isNextMoveStart;
        private Transform transform;
        private Mana manaComponent;
        private Entity chaseTarget;
        private Transform targetTransform;
        private HeavyWeaponAttack.HeavyWeaponAttackPipe heavyAttackPipe;
        private float maxChargeTime;
        private float maxGuardChargeTime;
        private float cycloneChanceIncrease;



        public override void InitializeFields(GameObject obj)
        {
            abilityRunner = controller.AbilityRunner;
            playerAbilityRunner = controller.PlayerAbilityRunner;
            trackData = controller.GetControllerData<AITrackData>();
            manaComponent = abilityRunner.GetAbilityStat<Mana>();
            transform = obj.transform;
            heavyAttackPipe = (HeavyWeaponAttack.HeavyWeaponAttackPipe)abilityRunner.GetAbilityPipe<HeavyWeaponAttack>();
        }

        public override void OnEnter()
        {
            controller.ChaseTarget();
            chaseTarget = controller.target;
            targetTransform = controller.target.transform;
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
                    if (!abilityRunner.IsAbilityRunning<Cyclone>()) //if cyclone is ready
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
                                Debug.Log("attack");
                                abilityRunner.EnqueueAbility<LightWeaponAttack>();
                            }
                            else
                            {
                                Debug.Log("attack");
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
                            Debug.Log("attack");
                            abilityRunner.EnqueueAbility<LightWeaponAttack>();
                        }
                        else
                        {
                            Debug.Log("attack");
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
            if (playerAbilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || playerAbilityRunner.IsAbilityRunning<LightWeaponAttack>()|| playerAbilityRunner.IsAbilityRunning<Cyclone>())//if player is attacking playerAbilityRunner.IsAbilityRunning<Cyclone>()
            {
                if (!abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() && !abilityRunner.IsAbilityRunning<LightWeaponAttack>()&& !abilityRunner.IsAbilityRunning<Cyclone>())//ai is not attacking playerAbilityRunner.IsAbilityRunning<Cyclone>()
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
/*            if(abilityRunner.IsAbilityRunning<Cyclone>())
            {
                return typeof(ChaseState);
            }*/
            if (controller.TargetInRange(attackRange) && isNextMoveStart==true)
            {
                GuardCheck();
            }
            if (abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || abilityRunner.IsAbilityRunning<LightWeaponAttack>()|| abilityRunner.IsAbilityRunning<Guard>()|| abilityRunner.IsAbilityRunning<Cyclone>()) 
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
