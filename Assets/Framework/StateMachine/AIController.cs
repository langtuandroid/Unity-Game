using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LobsterFramework.Utility;
using LobsterFramework.AbilitySystem;
using Pathfinding;
using LobsterFramework.ModifiedPathfinding;
using Pathfinding.Util;

namespace LobsterFramework.AI
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private List<ControllerData> controllerDatas;
        [SerializeField] private EntityGroup targetGroup;
        [SerializeField] private AbilityRunner abilityRunner;
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private AbilityRunner playerAbilityRunner;
        [SerializeField] private float visableDegree;
        public Entity target;
        private Transform _transform;
        private Collider2D _collider;
        private AIPathFinder pathFinder;
        private GridGraph gridGraph;
        private Dictionary<Type, ControllerData> controllerData;
        private Entity entityComponent;
        private MovementController moveControl;


        public AbilityRunner AbilityRunner { get { return abilityRunner; }}
        public AbilityRunner PlayerAbilityRunner { get { return playerAbilityRunner; } }
        public Entity GetEntity { get { return entityComponent; } }
        private void Awake()
        {
            gridGraph = AstarPath.active.data.gridGraph;
            controllerData = new();
            foreach (ControllerData data in controllerDatas)
            {
                controllerData[data.GetType()] = data;
            }
            entityComponent = GetComponent<Entity>();
            moveControl = GetComponent<MovementController>();
            _transform = GetComponent<Transform>();
            _collider = GetComponent<Collider2D>();
            pathFinder = GetComponent<AIPathFinder>();
        }

        private void OnEnable()
        {
            moveControl.onMovementBlocked += BlockMovement;
        }

        private void OnDisable()
        {
            moveControl.onMovementBlocked -= BlockMovement;
        }

        private void OnDrawGizmos()
        {
            if (_transform != null)
            {
                Draw.Gizmos.Line(_transform.position, pathFinder.Destination, Color.yellow);
            }
          
        }

        private void BlockMovement(bool value)
        {
            if (stateMachine != null)
            {
                stateMachine.enabled = !value;
            }
            if (pathFinder != null)
            {
                pathFinder.enabled = !value;
            }
        }

        public T GetControllerData<T>() where T : ControllerData
        {
            Type t = typeof(T);
            if (controllerData.ContainsKey(t))
            {
                return (T)controllerData[t];
            }
            return null;
        }

        public bool TargetInRange(float range)
        {
            return Vector3.Distance(_transform.position, target.transform.position) <= range;
        }

        public float GetTargetDistance()
        {
            return Vector3.Distance(_transform.position, target.transform.position);
        }

        public void ChaseTarget()
        {
            if (target != null)
            {
                pathFinder.SetTarget(target.transform);
            }
        }

        public void ResetTarget()
        {
            if(target != null)
            {
                pathFinder.Stop();
            }
            target = null;
        }
        public void stopChaseTarget()
        {
            if (target != null)
            {
                pathFinder.Stop();
            }
        }
        public bool SearchTarget(float sightRange)
        {
            Damage info = entityComponent.LatestDamageInfo;
            if (info.source != null)
            {
                target = info.source;
                /*Debug.Log("Received attack from target");*/
                return true;
            }
            RaycastHit2D hit = AIUtility.Raycast2D(gameObject,  _transform.position, _transform.up, sightRange, AIUtility.VisibilityMask);
            if (hit.collider != null)
            {
                Entity t = GameUtility.FindEntity(hit.collider.gameObject);
                if (t != null && targetGroup.Contains(t))
                {
                    target = t;
                    /*Debug.Log("AI: Target in range " + hit.distance);*/
                    return true;
                }
            }
            return false;
        }
        public bool TargetVisible(Vector3 position, float range)
        {
            float angle = Vector3.Angle(position, target.transform.position);
            if(angle > visableDegree || angle < -visableDegree)
            {
                return false;
            }
            RaycastHit2D hit = AIUtility.Raycast2D(gameObject, position, target.transform.position - position, range, AIUtility.VisibilityMask);
            if (hit.collider != null)
            {
                Entity t = hit.collider.GetComponent<Entity>();
                if (t != null && t == target)
                {
                    return true;
                }
            }
            return false;
        }
        public void PatrolLine(Vector3 postion)
        {
            pathFinder.MoveTowards(postion);
        }
        public void KeepDistanceFromTarget(Vector3 position, float distanceNeeded , float movedistance)
        {
            Quaternion rotation = Quaternion.AngleAxis(90, Vector3.forward);
            Vector3 vector = (position - target.transform.position).normalized* distanceNeeded;
            Vector3 tangengvector = rotation * vector;
            pathFinder.MoveTowards(tangengvector.normalized * movedistance + target.transform.position +vector);
        }
        public void Wander(int wanderRadius)
        {
            GraphNode startNode = gridGraph.GetNearest(_transform.position, NNConstraint.Default).node;
            List<GraphNode> nodes = PathUtilities.BFS(startNode, wanderRadius, filter: (GraphNode node) => { return PathUtilities.IsPathPossible(startNode, node); });
            if (nodes.Count > 0)
            {
                Vector3 dest = PathUtilities.GetPointsOnNodes(nodes, 1)[0];
                pathFinder.MoveTowards(dest);
            }
        }

        public void LookTowards()
        {
            moveControl.RotateTowards(target.transform.position - _transform.position);
        }

        public void MoveInDirection(Vector3 direction, float distance)
        {
            if (moveControl.MovementBlocked)
            {
                return;
            }
            float offset = _collider.bounds.size.x / 2;
            RaycastHit2D hit = Physics2D.Raycast(_transform.position, direction, distance + offset);
            if (hit.collider == null)
            {
                pathFinder.MoveTowards(_transform.position + direction.normalized * distance);
            }
            else
            {
                Vector2 dir = (Vector2)direction;
                pathFinder.MoveTowards(hit.point - dir.normalized * offset);
            }
        }

        public bool ReachedDestination()
        {
            return pathFinder.ReachedDestination;
        }
    }
}
