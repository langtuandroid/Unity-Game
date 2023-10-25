using System;
using UnityEngine;
using LobsterFramework;
using LobsterFramework.AbilitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;

namespace GameScripts.AI
{
    [CreateAssetMenu(menuName = "StateMachine/States/AI/Ranged-ChaseState")]
    public class RangedChaseState : State
    {
        private AITrackData trackData;
        private Transform transform;
        private AIController aiController;
        private Mana manaComponent;
        private AbilityRunner actionComponent;

        private Entity chaseTarget;
        private Transform targetTransform;

        public override void InitializeFields(GameObject obj)
        {
            aiController = obj.GetComponent<AIController>();
            actionComponent = obj.GetComponent<AbilityRunner>();
            manaComponent = actionComponent.GetAbilityStat<Mana>();
            trackData = aiController.GetControllerData<AITrackData>();
            transform = obj.transform;
        }

        public override void OnEnter()
        {
            chaseTarget = aiController.target;
            targetTransform = aiController.target.transform;
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
            if (aiController.TargetVisible(aiController.transform.position, aiController.transform.up, trackData.engageDistance.Value))
            {
                aiController.LookTowards();
                if (aiController.TargetInRange(trackData.keepDistance.Value))
                {
                    aiController.MoveInDirection(transform.position - targetTransform.position, trackData.keepDistance.Value - Vector3.Distance(transform.position, targetTransform.position));
                }
                actionComponent.EnqueueAbility<Shoot>();
                return null;
            }
            aiController.ChaseTarget();
            return null;
        }

        public override void OnExit()
        {
        }
    }
}
