using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class AIController : MonoBehaviour
{
    [SerializeField] private List<ControllerData> controllerDatas;
    [SerializeField] private EntityGroup targetGroup;
    [SerializeField] private RefInt rotateSpeed;

    public Entity target;
    private Transform _transform;
    private Collider2D _collider;

    private AIPath ai;
    private Seeker seeker;
    private GridGraph gridGraph;

    private Dictionary<Type, ControllerData> controllerData;
    private Entity entityComponent;
    private bool isRotating;
    private Quaternion endGoal;

    private void Awake()
    {
        _transform = transform;
        _collider = GetComponent<Collider2D>();
        ai = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();
        gridGraph = AstarPath.active.data.gridGraph;
        controllerData = new();
        foreach (ControllerData data in controllerDatas) {
            controllerData[data.GetType()] = data;  
        }
        entityComponent = GetComponent<Entity>();
    }

    private void Update()
    {
        Debug.DrawLine(_transform.position, ai.destination, Color.green);
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
        DamageInfo info = entityComponent.LatestDamageInfo;
        if (info.attacker != null) {
            target = info.attacker;
            Debug.Log("Received attack from target");
            return true;
        }
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
        List<GraphNode> nodes = PathUtilities.BFS(startNode, wanderRadius, filter: (GraphNode node) => { return PathUtilities.IsPathPossible(startNode, node); });
        if (nodes.Count > 0) {
            Vector3 dest = PathUtilities.GetPointsOnNodes(nodes, 1)[0];
            ai.destination = dest;
        }
    }

    public void LookTowards() {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - _transform.position);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, rotateSpeed.Value * Time.deltaTime);
    }

    public void BackStep()
    {
        ai.Move(( - _transform.up).normalized * ai.maxSpeed / 2 * Time.deltaTime);
    }

    public void MoveInDirection(Vector3 direction, float distance) {
        float offset = _collider.bounds.size.x / 2;
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, direction, distance + offset);
        if (hit.collider == null)
        {
            ai.destination = _transform.position + direction.normalized * distance;
        }
        else {
            Vector2 dir = (Vector2)direction;
            ai.destination = hit.point - dir.normalized * offset;
        }
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
