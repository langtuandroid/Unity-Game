using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework
{
    public class ContinousCollision2D
    {
        private Transform transform;
        private LayerMask collisionLayers;
        private Vector3 previousPos;

        public ContinousCollision2D(Transform transform, LayerMask collisionLayer) {
            this.transform = transform;
            collisionLayers = collisionLayer;
            previousPos = transform.position;
        }

        public void Reset() {
            previousPos = transform.position;
        }

        public SimpleCollision2D HasCollision() {
            Vector3 ray = transform.position - previousPos;
            RaycastHit2D hit = Physics2D.Raycast(previousPos, ray, ray.magnitude, collisionLayers);
            previousPos = transform.position;
            return new SimpleCollision2D { collider = hit.collider, contactPoint = hit.point };
        }

        public void Draw() {
            Gizmos.DrawLine(previousPos, transform.position);
        }
    }

    public struct SimpleCollision2D
    {
        public Collider2D collider;
        public Vector3 contactPoint;
    }
}
