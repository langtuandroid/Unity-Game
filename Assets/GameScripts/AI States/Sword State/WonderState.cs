using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AI;
using System.Security.Cryptography;
using LobsterFramework;

namespace GameScripts.AI.SwordEnemy
{
    [CreateAssetMenu(menuName = "StateMachine/States/AI/SwordEnemy/SwordWanderState")]
    public class WanderState : State
    {
        [SerializeField] private RefFloat wanderTime;
        [SerializeField] private RefInt wanderRadius;
        [SerializeField] private RefFloat idleTime;
        private StealthController trans;
        private float wanderTimeCounter;
        private float idleTimeCounter;
        private Rigidbody2D daggerRigid;
        private MovementController moveControl;
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
            transform = controller.transform;
            trans = controller.GetComponent<StealthController>();
            moveControl = controller.GetComponent<MovementController>();
            daggerRigid = controller.GetComponent<Rigidbody2D>();
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
                /*float enemyHealth = controller.target.Health / controller.target.MaxHealth;
                if (enemyHealth>0.4)
                {
                    Debug.Log("fuck u");
                    return typeof(MeleeChaseState);
                    //return typeof(RangedChaseState);
                }
                else
                {
                    float meleeDesire = 0.3f;
                    float diffinHealth = 0.4f - enemyHealth;
                    meleeDesire += diffinHealth / 0.4f * 0.7f;
                    float randomNumber = UnityEngine.Random.Range(0f, 1f);
                    if(randomNumber > meleeDesire)
                    {
                        Debug.Log("fuck u");
                        //return typeof(RangedChaseState);
                    }
                    return typeof(MeleeChaseState);
                }*/
                return typeof(ChaseState);

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
                    if(daggerRigid.velocity!=Vector2.zero)
                    {
                        moveControl.RotateTowards(daggerRigid.velocity);
                    }

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
