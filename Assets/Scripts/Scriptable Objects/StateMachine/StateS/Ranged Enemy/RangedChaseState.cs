using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Action;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.AI
{
    [CreateAssetMenu(menuName = "StateMachine/States/AI/Ranged-ChaseState")]
    public class RangedChaseState : State
    {
        private AITrackData trackData;
        private Transform transform;
        private AIController aiController;
        private Mana manaComponent;
        private Actionable actionComponent;

        private Entity chaseTarget;
        private Transform targetTransform;

        public override void InitializeFields(GameObject obj)
        {
            aiController = obj.GetComponent<AIController>();
            actionComponent = obj.GetComponent<Actionable>();
            manaComponent = actionComponent.GetActionComponent<Mana>();
            trackData = aiController.GetControllerData<AITrackData>();
            transform = obj.transform;
        }

        public override void OnEnter()
        {
            chaseTarget = aiController.target;
            targetTransform = aiController.target.transform;
        }

        public override void OnExit()
        {
            aiController.AutoRotation = true;
        }

        public bool InChaseRange()
        {
            return Vector3.Distance(transform.position, chaseTarget.transform.position) <= trackData.chaseDistance.Value;
        }

        public override Type Tick()
        {
            if (!aiController.target.gameObject.activeInHierarchy || !aiController.TargetInRange(trackData.chaseDistance.Value))
            {
                aiController.target = null;
                return typeof(WanderState);
            }
            Debug.DrawLine(transform.position, (aiController.target.transform.position - transform.position).normalized * trackData.engageDistance.Value + transform.position, Color.red);
            if (aiController.TargetVisible(aiController.transform.position, trackData.engageDistance.Value))
            {
                aiController.AutoRotation = false;
                aiController.LookTowards();
                if (aiController.TargetInRange(trackData.keepDistance.Value))
                {
                    aiController.MoveInDirection(transform.position - targetTransform.position, trackData.keepDistance.Value - Vector3.Distance(transform.position, targetTransform.position));
                }
                actionComponent.EnqueueAction<Shoot>();
                return null;
            }
            aiController.AutoRotation = true;
            aiController.ChaseTarget();
            return null;
        }
    }
}
