using System;
using UnityEngine;
using LobsterFramework.AI;

namespace GameScripts.AI
{
    [CreateAssetMenu(menuName = "StateMachine/States/AI/WanderState2")]
    public class Wander2 : State
    {
        private AIController controller;
        [SerializeField] private RefFloat wanderTime;
        [SerializeField] private RefInt wanderRadius;
        [SerializeField] private RefFloat idleTime;

        private float wanderTimeCounter;
        private float idleTimeCounter;
        private Transform transform;
        private AITrackData trackingData;

        private WanderInternalState wanderState;

        private enum WanderInternalState
        {
            Idle,
            PathFinding,
            Wandering
        }


        public override void InitializeFields(GameObject obj)
        {
            controller = obj.GetComponent<AIController>();
            transform = obj.transform;
            trackingData = controller.GetControllerData<AITrackData>();
        }

        public override void OnEnter()
        {
            wanderState = WanderInternalState.Idle;
            wanderTimeCounter = 0;
            idleTimeCounter = 0;
        }

        public override void OnExit()
        {

        }

        public override Type Tick()
        {
            float sight = trackingData.sightRange.Value;
            Debug.DrawLine(transform.position, transform.position + transform.up * sight, Color.yellow);
            if (controller.SearchTarget(sight))
            {
                return typeof(MeleeChaseState2);
            }
            switch (wanderState)
            {
                case WanderInternalState.Idle:
                    idleTimeCounter += Time.deltaTime;
                    if (idleTimeCounter >= idleTime.Value)
                    {
                        idleTimeCounter = 0;
                        wanderState = WanderInternalState.PathFinding;
                    }
                    break;
                case WanderInternalState.PathFinding:
                    controller.Wander(wanderRadius.Value);
                    wanderState = WanderInternalState.Wandering;
                    break;
                case WanderInternalState.Wandering:
                    wanderTimeCounter += Time.deltaTime;
                    if (controller.ReachedDestination() || wanderTimeCounter >= wanderTime.Value)
                    {
                        wanderTimeCounter = 0;
                        wanderState = WanderInternalState.Idle;
                    }
                    break;
            }
            return null;
        }
    }
}
