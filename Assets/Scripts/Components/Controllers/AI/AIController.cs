using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class AIController : MonoBehaviour
{
    [SerializeField] private List<ControllerData> controllerDatas;
    [SerializeField] private EntityGroup targetGroup;

    public Entity target;
    private Transform _transform;

    private AIPath ai;
    private GridGraph gridGraph;

    private Dictionary<Type, ControllerData> controllerData;

    private void Awake()
    {
        _transform = transform;
        ai = GetComponent<AIPath>();
        gridGraph = AstarPath.active.data.gridGraph;
        controllerData = new();
        foreach (ControllerData data in controllerDatas) {
            controllerData[data.GetType()] = data;  
        }
    }

    private void Update()
    {

    }

    public T GetControllerData<T>() where T : ControllerData{
        Type t = typeof(T);
        if (controllerData.ContainsKey(t)) {
            return (T)controllerData[t];
        }
        return null;
    }
    public bool TargetInRange(float range) {
        return Vector3.Distance(_transform.position, target.transform.position) <= range;
    }

    public float GetTargetDistance() {
        return Vector3.Distance(_transform.position, target.transform.position);
    }

    public void ChaseTarget()
    {
        if (target != null) {
            ai.destination = target.transform.position;
        }
    }

    public bool SearchTarget(float sightRange) {
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, _transform.up, sightRange);
        if (hit.collider != null) {
            Entity t = hit.collider.GetComponent<Entity>();
            if (t != null && targetGroup.Contains(t)) {
                target = t;
                Debug.Log("AI: Target in range " + hit.distance);
                return true;
            }
        }
        return false;
    }

    public bool TargetVisible(Vector3 position, float range) {
        RaycastHit2D hit = Physics2D.Raycast(position, target.transform.position - position, range);
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

    public void Wander(int wanderRadius) {
        GraphNode startNode = gridGraph.GetNearest(_transform.position, NNConstraint.Default).node;
        List<GraphNode> nodes = PathUtilities.BFS(startNode, wanderRadius, filter: (GraphNode node) => { return node.Walkable; });
        if (nodes.Count > 0) {
            Vector3 dest = PathUtilities.GetPointsOnNodes(nodes, 1)[0];
            ai.destination = dest;
        }
    }

    public void LookAtTarget() {
        _transform.up = target.transform.position - _transform.position;
    }

    public void BackStep()
    {
        ai.Move(( - _transform.up).normalized * ai.maxSpeed * Time.deltaTime);
    }

    public void ClearPath() {
        ai.SetPath(null);
        ai.destination = _transform.position;
    }

    public Vector3 NextWayPoint() {
        return ai.steeringTarget;
    }

    public bool StopPathing {
        get { return ai.isStopped; }
        set { ai.isStopped = value; }
    }

    public bool AutoRotation {
        get { return ai.enableRotation; }
        set { ai.enableRotation = value; }
    }

    public bool ReachedDestination() {
        return ai.reachedDestination;
    }
}
