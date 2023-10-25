using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AI;
using System.Security.Cryptography;
using LobsterFramework;
using UnityEngine.InputSystem.XR;
using LobsterFramework.AbilitySystem;
using GameScripts.Abilities;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace GameScripts.AI.DaggerEnemy
{
    [CreateAssetMenu(menuName = "StateMachine/States/AI/DaggerEnemy/DaggerWanderState")]
    public class WanderState : State
    {
        [SerializeField] private RefFloat wanderTime;
        [SerializeField] private RefInt wanderRadius;
        [SerializeField] private RefFloat idleTime;
        [SerializeField] private List<Vector3> PatrolPoint;
        [SerializeField] private float holdTime=3f;
        [SerializeField] private float dangerDistance = 3f;
        private EntityAlertBar alertBar;
        private StealthController trans;
        private float wanderTimeCounter;
        private float idleTimeCounter;
        private Rigidbody2D daggerRigid;
        private MovementController moveControl;
        private Transform transform;
        private AITrackData trackingData;
        private WanderInternalState wanderState;
        private int PatrolNum;
        private int currentPatrolNum;
        private float maxholdTime;
        private float currentHealth;
        private float pastHealth;

        private enum WanderInternalState
        {
            Idle,
            PathFinding,
            Wandering,
        }


        public override void InitializeFields(GameObject obj)
        {
            transform = controller.transform;
            trans = controller.GetComponent<StealthController>();
            moveControl = controller.GetComponent<MovementController>();
            daggerRigid = controller.GetComponent<Rigidbody2D>();
            trackingData = controller.GetControllerData<AITrackData>();
            currentPatrolNum = 0;
            PatrolNum = PatrolPoint.Count;
            currentHealth = controller.GetEntity.Health;
            pastHealth = controller.GetEntity.Health;
            alertBar = controller.GetUtil<EntityAlertBar>();
        }

        public override void OnEnter()
        {
            wanderState = WanderInternalState.Idle;
            wanderTimeCounter = 0;
            idleTimeCounter = 0;
            maxholdTime = 0;
        }

        public override void OnExit()
        {

        }
        protected IEnumerator<CoroutineOption> HoldPostion(float sight)
        {
            pastHealth = controller.GetEntity.Health;
            maxholdTime = Time.time + holdTime;
            while (Time.time < maxholdTime)
            {
                currentHealth = controller.GetEntity.Health;
                if (currentHealth != pastHealth || controller.GetTargetDistance() < dangerDistance)
                {
                    alertBar.Hide();
                    maxholdTime = -1;
                    break;
                }
                pastHealth = currentHealth;
                if (controller.TargetVisible(controller.transform.position,controller.transform.up, trackingData.engageDistance.Value))
                {
                    alertBar.SetAlert(Time.deltaTime / holdTime);
                    yield return null;
                }
                else
                {
                    alertBar.Hide();
                    maxholdTime = 0;
                    break;
                }
                
            }
        }
        public override Type Tick()
        {
            
            float sight = trackingData.sightRange.Value;
            Debug.DrawLine(transform.position, transform.position + transform.up * sight, Color.yellow);
            currentHealth = controller.GetEntity.Health;
            if (controller.SearchTarget(sight))
            {
                
                if (currentHealth != pastHealth || controller.GetTargetDistance() < dangerDistance)
                {
                    return typeof(ChaseState);
                }
                pastHealth = currentHealth;
                stateMachine.RunCoroutine(HoldPostion(sight));
                alertBar.Hide();
                if (maxholdTime!=0)
                {
                    if (Time.time > maxholdTime || maxholdTime == -1)
                    {
                        return typeof(ChaseState);
                    } 
                }    
            }
            switch (wanderState)
            {
                case WanderInternalState.Idle:
                    trans.Changetrans(0.3f);
                    idleTimeCounter += Time.deltaTime;
                    if (idleTimeCounter >= idleTime.Value)
                    {
                        idleTimeCounter = 0;
                        if(currentPatrolNum< PatrolNum-1)
                        {
                            currentPatrolNum += 1;
                        }
                        else
                        {
                            currentPatrolNum = 0;
                        }
                        
                        wanderState = WanderInternalState.PathFinding;
                    }
                    break;
                case WanderInternalState.PathFinding:
                    trans.Changetrans(0.3f);
                    if (PatrolPoint[currentPatrolNum] != null)
                    {
                        controller.PatrolLine(PatrolPoint[currentPatrolNum]);
                    }
                    wanderState = WanderInternalState.Wandering;
                    break;
                case WanderInternalState.Wandering:
                    trans.Changetrans(0.3f);
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
