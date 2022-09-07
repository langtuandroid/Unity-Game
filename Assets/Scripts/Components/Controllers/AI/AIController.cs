using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIController : MonoBehaviour
{
    [SerializeField] private RefFloat sightRange;
    [SerializeField] private RefFloat chaseRadius;
    [SerializeField] private RefInt wanderRadius;
    [SerializeField] private EntityGroup targetGroup;

    public Entity target;
    private Transform _transform;

    private AIPath ai;
    private GridNode randomNode;
    private GridGraph gridGraph;

    private void Start()
    {
        _transform = transform;
        ai = GetComponent<AIPath>();
        gridGraph = AstarPath.active.data.gridGraph;
    }

    private void Update()
    {
        Debug.DrawLine(_transform.position, _transform.position + _transform.up * sightRange.Value, Color.yellow);
    }

    public bool InChaseRange() { 
        return Vector3.Distance(_transform.position, target.transform.position) <= chaseRadius.Value;
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

    public bool TargetInSignt() {
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, _transform.up, sightRange.Value);
        if (hit.collider != null) {
            Entity t = hit.collider.GetComponent<Entity>();
            if (t != null && targetGroup.Contains(t)) {
                target = t;
                Debug.Log(hit.distance);
                return true;
            }
        }
        return false;
    }

    public void Wander() {
        GraphNode startNode = gridGraph.GetNearest(_transform.position, NNConstraint.Default).node;
        List<GraphNode> nodes = PathUtilities.BFS(startNode, wanderRadius.Value, filter: (GraphNode node) =>{ return node.Walkable; });
        if (nodes.Count > 0) {
            Vector3 dest = PathUtilities.GetPointsOnNodes(nodes, 1)[0];
            ai.destination = dest;
        }
    }

    public bool ReachedDestination() {
        return ai.reachedDestination;
    }
}
