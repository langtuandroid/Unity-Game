using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;

namespace GameScripts.AI
{

    [CreateAssetMenu(menuName = "StateMachine/States/AI/Melee-ChaseState")]
    public class MeleeChaseState : State
    {
        [SerializeField] private float attackRange;
        private AITrackData trackData;
        private AbilityRunner abilityRunner;


        public override void InitializeFields(GameObject obj)
        {
            abilityRunner = controller.AbilityRunner;
            trackData = controller.GetControllerData<AITrackData>();
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
            controller.target = null;
        }

        public override Type Tick()
        {
            if (!controller.target.gameObject.activeInHierarchy)
            {
                return typeof(WanderState);
            } 

            if (controller.TargetInRange(trackData.chaseDistance.Value))
            {
                if (controller.TargetInRange(attackRange))
                {
                    abilityRunner.EnqueueAbility<LightWeaponAttack>();
                }
                else {
                    controller.ChaseTarget();
                }
                return null;
            }
            return typeof(WanderState);
        }
    }
}   
