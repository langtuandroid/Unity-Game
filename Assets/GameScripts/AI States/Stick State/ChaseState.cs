using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;
using LobsterFramework;
using static UnityEngine.EventSystems.EventTrigger;
using static LobsterFramework.AbilitySystem.Ability;
using System.Collections.Generic;
using Codice.CM.Client.Differences.Merge;

namespace GameScripts.AI.StickEnemy
{

    [CreateAssetMenu(menuName = "StateMachine/States/AI/StickEnemy/ChaseState")]
    public class ChaseState : State
    {
        [Header("Behaviour Stats")]
        [Range(0, 10)][SerializeField] private float backKeepDistance;
        [Range(0, 1)][SerializeField] private float postureThreshold;
        [Range(0, 1)][SerializeField] private float cycloneProb;
        [Range(0, 1)][SerializeField] private float lightAttackProb;
        [Range(0, 1)][SerializeField] private float guardProb;
        [Range(0, 1)][SerializeField] private float cycloneChanceIncrease;
        [Range(0, 1)][SerializeField] private float guardChanceDecrease;
        [SerializeField] private float attackRange;
        private AITrackData trackData;
        private AbilityRunner abilityRunner;
        private AbilityRunner playerAbilityRunner;
        private bool isNextMoveStart;
        private Transform transform;
        private HeavyWeaponAttack.HeavyWeaponAttackPipe heavyAttackPipe;
        
        private float posture;
        private float moveDistance;
        private bool isResting;
        public override void InitializeFields(GameObject obj)
        {
            abilityRunner = controller.AbilityRunner;
            playerAbilityRunner = controller.PlayerAbilityRunner;
            trackData = controller.GetControllerData<AITrackData>();
            transform = obj.transform;
            heavyAttackPipe = (HeavyWeaponAttack.HeavyWeaponAttackPipe)abilityRunner.GetAbilityPipe<HeavyWeaponAttack>();
        }

        public override void OnEnter()
        {
            controller.ChaseTarget();
            isNextMoveStart = false;
            playerAbilityRunner.onAbilityEnqueued += OnPlayerAction;
            posture = controller.target.Posture / controller.target.MaxPosture;
            moveDistance = 0;
            isResting = false;
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

                        if (cycloneChance > cycloneProb - cycloneChanceIncrease) //if cyclone
                        {
                            cycloneChanceIncrease = 0;
                            abilityRunner.EnqueueAbility<WeaponArt>();
                        }
                        else
                        {
                            cycloneChanceIncrease += 0.2f;
                            float attackType = UnityEngine.Random.Range(0f, 1f);
                            if (attackType > lightAttackProb)
                            {
                                abilityRunner.EnqueueAbility<LightWeaponAttack>();
                            }
                            else
                            {
                                stateMachine.RunCoroutine(HeavyAttackTime());
                                return null;
                            }
                        }
                    }
                    else
                    {
                        cycloneChanceIncrease += 0.2f;
                        float attackType = UnityEngine.Random.Range(0f, 1f);
                        if (attackType > lightAttackProb)
                        {
                            abilityRunner.EnqueueAbility<LightWeaponAttack>();
                        }
                        else
                        {
                            stateMachine.RunCoroutine(HeavyAttackTime());
                            return null;
                        }
                    }

                }
                return null;
            }
            return null;
        }
        protected IEnumerator<CoroutineOption> HeavyAttackTime()
        {
            abilityRunner.EnqueueAbility<HeavyWeaponAttack>();
            float randomChargeTime = UnityEngine.Random.Range(0.5f, 1.5f);
            yield return CoroutineOption.WaitForSeconds(randomChargeTime * heavyAttackPipe.MaxChargeTime);
            abilityRunner.Signal<HeavyWeaponAttack>();
        }
        protected IEnumerator<CoroutineOption> GuardTime()
        {
            Debug.Log("guard");
            abilityRunner.EnqueueAbility<Guard>();
            float randomGuardChargeTime = UnityEngine.Random.Range(0.5f, 1f);
            yield return CoroutineOption.WaitForSeconds(randomGuardChargeTime);
            abilityRunner.Signal<Guard>();
        }
        public Type GuardCheck()
        {
            isNextMoveStart = false;
            if (playerAbilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || playerAbilityRunner.IsAbilityRunning<LightWeaponAttack>()|| playerAbilityRunner.IsAbilityRunning<WeaponArt>()|| playerAbilityRunner.IsAbilityRunning<Shoot>())//if player is attacking
            {
                if (!abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() && !abilityRunner.IsAbilityRunning<LightWeaponAttack>()&& !abilityRunner.IsAbilityRunning<WeaponArt>())//ai is not attacking player
                {
                    float GuardChance = UnityEngine.Random.Range(0f, 1f);
                    if (GuardChance < guardProb)
                    {
                        stateMachine.RunCoroutine(GuardTime());
                    }
                }
                else
                {
                    float GuardChance = UnityEngine.Random.Range(0f, 1f);
                    if (GuardChance < guardProb - guardChanceDecrease)
                    {
                        stateMachine.RunCoroutine(GuardTime());
                    }
                }
            }
            return null;
        }
        protected IEnumerator<CoroutineOption> RestTime()
        {
            float maxRestTime = Time.time + UnityEngine.Random.Range(3f, 5f);
            while(Time.time < maxRestTime)
            {
                moveDistance = UnityEngine.Random.Range(-1f, 1f);
                controller.KeepDistanceFromTarget(transform.position, backKeepDistance, moveDistance);
                float walkFinishTime = UnityEngine.Random.Range(0.5f, 1f) + Time.time;
                while (Time.time < walkFinishTime)
                {
                    controller.LookTowards();
                    if (isNextMoveStart == true)
                    {
                        isNextMoveStart = false;
                        if (playerAbilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || playerAbilityRunner.IsAbilityRunning<LightWeaponAttack>() || playerAbilityRunner.IsAbilityRunning<WeaponArt>() || playerAbilityRunner.IsAbilityRunning<Shoot>())//if player is attacking
                        {
                            yield return CoroutineOption.WaitForCoroutine(GuardTime());
                        }
                    }
                    yield return null;

                }
                abilityRunner.Signal<Guard>();
                yield return null;
            }
            controller.ChaseTarget();
        }
        public override Type Tick()
        {
            if (!controller.target.gameObject.activeInHierarchy || !controller.TargetInRange(trackData.chaseDistance.Value))
            {
                controller.ResetTarget();
                return typeof(WanderState);
            }
            controller.LookTowards();
            posture = controller.GetEntity.Posture / controller.GetEntity.MaxPosture;
            if (posture< postureThreshold)
            {
                stateMachine.RunCoroutine(RestTime());
                
            }
            if (controller.TargetInRange(attackRange) && isNextMoveStart==true)
            {
                GuardCheck();
            }
            if (abilityRunner.IsAbilityRunning<HeavyWeaponAttack>() || abilityRunner.IsAbilityRunning<LightWeaponAttack>()|| abilityRunner.IsAbilityRunning<Guard>()|| abilityRunner.IsAbilityRunning<WeaponArt>()) 
            {
                return null;
            }
            MeleeAttack();
            return null;
        }
    }
}   
