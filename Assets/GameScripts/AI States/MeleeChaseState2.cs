using System;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.AI;
using GameScripts.Abilities;

namespace GameScripts.AI
{
    [CreateAssetMenu(menuName = "StateMachine/States/AI/Melee-ChaseState2")]
    public class MeleeChaseState2 : State
    {
        private AITrackData trackData;
        private Transform transform;
        private AIController aiController;
        private CombatStat combatComponent;
        private AbilityRunner abilityRunner;

        private Entity chaseTarget;

        public override void InitializeFields(GameObject obj)
        {
            aiController = obj.GetComponent<AIController>();
            abilityRunner = obj.GetComponent<AbilityRunner>();
            combatComponent = abilityRunner.GetAbilityStat<CombatStat>();
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
                    abilityRunner.EnqueueAbility<RightSwipe>();
                }
                return null;
            }
            return typeof(Wander2);
        }
    }
}
