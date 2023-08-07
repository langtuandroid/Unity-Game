using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.ModifiedPathfinding
{
    public class AIPathFinder : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Vector3 targetPosition;

        #region Components
        private Seeker seeker;
        private new Transform transform;
        private MovementController moveControl;
        #endregion

        private Path path;

        public float nextWaypointDistance = 0.5f;

        private int currentWaypoint = 0;
        private bool reachedEndOfPath;

        private float lastPathed;
        [SerializeField] private float repathInterval;



        public Vector3 Destination { get { return targetPosition; } }
        public bool ReachedDestination { get { return path == null; } }

        public void Start()
        {
            seeker = GetComponent<Seeker>();
            transform = GetComponent<Transform>();
            moveControl = GetComponent<MovementController>();
            // If you are writing a 2D game you should remove this line
            // and use the alternative way to move sugggested further below.
        }

        public void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                // Reset the waypoint counter so that we start to move towards the first point in the path
                currentWaypoint = 0;
                path.Claim(this);
            }
        }

        public void MoveTowards(Vector3 position) {
            if (path != null) {
                path.Release(this);
                path = null;
            }
            seeker.StartPath(transform.position, position, OnPathComplete);
            lastPathed = Time.time;
            targetPosition = position;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void Stop()
        {
            if(path != null)
            {
                target = null;
                path.Release(this);
                path = null;
            }
        }

        public void Update()
        {
            if (lastPathed + repathInterval <= Time.time && target != null)
            {
                MoveTowards(target.transform.position);
                return;
            }
            if (path == null)
            {
                // We have no path to follow yet, so don't do anything
                return;
            }
            // Check in a loop if we are close enough to the current waypoint to switch to the next one.
            // We do this in a loop because many waypoints might be close to each other and we may reach
            // several of them in the same frame.
            reachedEndOfPath = false;
            // The distance to the next waypoint in the path
            float distanceToWaypoint;
            while (true)
            {
                // If you want maximum performance you can check the squared distance instead to get rid of a
                // square root calculation. But that is outside the scope of this tutorial.
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    // Check if there is another waypoint or if we have reached the end of the path
                    if (currentWaypoint + 1 < path.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        // Set a status variable to indicate that the agent has reached the end of the path.
                        // You can use this to trigger some special code if your game requires that.
                        reachedEndOfPath = true;
                        path.Release(this);
                        path = null;
                        return;
                    }
                }
                else
                {
                    break;
                }
            }

            // Slow down smoothly upon approaching the end of the path
            // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

            // Direction to the next waypoint
            // Normalize it so that it has a length of 1 world unit
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            // Multiply the direction by our desired speed to get a velocity
            Vector3 velocity = moveControl.Speed * speedFactor * dir;

            // Move the agent using the CharacterController component
            // Note that SimpleMove takes a velocity in meters/second, so we should not multiply by Time.deltaTime
            moveControl.SetVelocity(velocity);
        }
    }
}
