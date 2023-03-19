using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Action;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.AI
{
    [CreateAssetMenu(menuName = "StateMachine/States/AI/Melee-ChaseState2")]
    public class MeleeChaseState2 : State
    {
        private AITrackData trackData;
        private Transform transform;
        private AIController aiController;
        private CombatComponent combatComponent;
        private Actionable actionComponent;

        private Entity chaseTarget;

        public override void InitializeFields(GameObject obj)
        {
            aiController = obj.GetComponent<AIController>();
            actionComponent = obj.GetComponent<Actionable>();
            combatComponent = actionComponent.GetActionComponent<CombatComponent>();
            trackData = aiController.GetControllerData<AITrackData>();
        }

        public override void OnEnter()
        {
            chaseTarget = aiController.target;
        }

        public override void OnExit()
        {
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
                return typeof(Wander2);
            }
            aiController.ChaseTarget();
            if (aiController.TargetInRange(trackData.chaseDistance.Value))
            {
                if (aiController.TargetInRange(combatComponent.attackRange.Value))
                {
                    actionComponent.EnqueueAction<RightSwipe>();
                }
                return null;
            }
            return typeof(Wander2);
        }
    }
}
