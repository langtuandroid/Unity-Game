using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LobsterFramework
{
    public class AIUtility : MonoBehaviour
    {
        [Tooltip("LayerMask used by AI to detect player by raycasting")]
        [SerializeField] private LayerMask visibilityMask;
        [SerializeField, Layer] private int noCollisionLayer;
        private static AIUtility instance;

        public static LayerMask VisibilityMask { get { return instance.visibilityMask; } }
        public static int NoCollisionLayer { get { return instance.noCollisionLayer; } }

        public static RaycastHit2D Raycast2D(GameObject caster, Vector3 position, Vector2 direction, float range, LayerMask layerMask) {
            int layer = caster.layer;
            caster.layer = NoCollisionLayer;
            RaycastHit2D hit = Physics2D.Raycast(position, direction, range, layerMask);
            caster.layer = layer;
            return hit;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
