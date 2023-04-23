using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.AI
{

    [CreateAssetMenu(menuName = "StateMachine/States/AI/Melee-ChaseState")]
    public class MeleeChaseState : State
    {
        private AITrackData trackData;
        private Transform transform;
        private AIController aiController;
        private CombatStat combatComponent;
        private AbilityRunner actionComponent;

        private AbilityRunner targetAction;
        private CombatStat targetCombatComponent;

        private Entity chaseTarget;

        public override void InitializeFields(GameObject obj)
        {
            aiController = obj.GetComponent<AIController>();
            actionComponent = obj.GetComponent<AbilityRunner>();
            combatComponent = actionComponent.GetAbilityStat<CombatStat>();
            trackData = aiController.GetControllerData<AITrackData>();
        }

        public override void OnEnter()
        {
            chaseTarget = aiController.target;
            targetAction = chaseTarget.GetComponent<AbilityRunner>();
            if (targetAction != null)
            {
                targetCombatComponent = targetAction.GetAbilityStat<CombatStat>();
            }
        }

        public override void OnExit()
        {
            targetCombatComponent = null;
            aiController.target = null;
        }

        public bool InChaseRange()
        {
            return Vector3.Distance(transform.position, chaseTarget.transform.position) <= trackData.chaseDistance.Value;
        }

        public override Type Tick()
        {
            if (!aiController.target.gameObject.activeInHierarchy)
            {
                return typeof(WanderState);
            }
            aiController.ChaseTarget();
            if (aiController.TargetInRange(trackData.chaseDistance.Value))
            {
                if (aiController.TargetInRange(combatComponent.attackRange.Value))
                {
                    actionComponent.EnqueueAbility<CircleAttack>();
                }
                if (IsVulnerableToMelee())
                {
                    actionComponent.EnqueueAbility<Guard>();
                }
                return null;
            }
            return typeof(WanderState);
        }

        private bool IsVulnerableToMelee()
        {
            if (targetAction == null)
            {
                return false;
            }
            return targetAction.IsAbilityReady<CircleAttack>() && aiController.TargetInRange(targetCombatComponent.attackRange.Value);
        }
    }
}   
