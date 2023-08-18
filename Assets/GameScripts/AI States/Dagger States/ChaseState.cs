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
        private AITrackData trackData;
        private AbilityRunner abilityRunner;
        float meleeOn = 0;
        private Transform transform;
        private Mana manaComponent;
        private AbilityRunner actionComponent;
        private Entity chaseTarget;
        private Transform targetTransform;

        public override void InitializeFields(GameObject obj)
        {
            abilityRunner = controller.AbilityRunner;
            trackData = controller.GetControllerData<AITrackData>();
            actionComponent = obj.GetComponent<AbilityRunner>();
            manaComponent = actionComponent.GetAbilityStat<Mana>();
            transform = obj.transform;
        }

        public override void OnEnter()
        {
            Debug.Log("Chase");
            controller.ChaseTarget();
            chaseTarget = controller.target;
            targetTransform = controller.target.transform;
            meleeOn = 0;
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
            controller.LookTowards();
            if (controller.TargetInRange(trackData.chaseDistance.Value))
            {
                if (controller.TargetInRange(attackRange))
                {
                    float randomNumber = UnityEngine.Random.Range(0f, 1f);
                    Debug.Log(randomNumber);
                    if (randomNumber > 0.7)
                    {
                        abilityRunner.EnqueueAbility<LightWeaponAttack>();
                    }
                    else
                    {
                        abilityRunner.EnqueueAbility<HeavyWeaponAttack>();
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
            controller.LookTowards();
            Debug.DrawLine(transform.position, (controller.target.transform.position - transform.position).normalized * trackData.engageDistance.Value + transform.position, Color.red);
            if (controller.TargetVisible(controller.transform.position, trackData.engageDistance.Value))
            {

                if (controller.TargetInRange(trackData.keepDistance.Value))
                {
                    controller.MoveInDirection(transform.position - targetTransform.position, trackData.keepDistance.Value - Vector3.Distance(transform.position, targetTransform.position));
                }
                actionComponent.EnqueueAbility<Shoot>();
                return typeof(ChaseState);
            }
            controller.ChaseTarget();
            return typeof(ChaseState);
        }

        public override Type Tick()
        {
            if(!abilityRunner.IsAbilityReady<HeavyWeaponAttack>()|| !abilityRunner.IsAbilityReady<LightWeaponAttack>())
            {
                return null;
            }
            if (!controller.target.gameObject.activeInHierarchy|| !controller.TargetInRange(trackData.chaseDistance.Value))
            {
                controller.ResetTarget();
                return typeof(WanderState);
            }
            float enemyHealth = controller.target.Health / controller.target.MaxHealth;
            if (enemyHealth > 0.4 && meleeOn!=1)
            {
                Debug.Log("shoot");

                RangeAttack();
            }
            else
            {
                
                float meleeDesire = 0.3f;
                float diffinHealth = 0.4f - enemyHealth;
                meleeDesire += diffinHealth / 0.4f * 0.7f;
                float randomNumber = UnityEngine.Random.Range(0f, 1f);
                if (randomNumber > meleeDesire && meleeOn != 1)
                {
                    Debug.Log("aaaaa");
                    RangeAttack();
                }
                meleeOn = 1;
                Debug.Log("melee");
                MeleeAttack();
                
            }
            return null;
        }
    }
}   
