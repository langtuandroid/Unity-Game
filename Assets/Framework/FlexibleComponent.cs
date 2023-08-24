using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework
{
    public class FlexibleComponent : MonoBehaviour
    {
        [SerializeField] private Transform topLevelTransform;

        public Transform TopLevelTransform { get { return topLevelTransform; } }

        public T GetTopLevelComponent<T>() where T : Component {
            if (topLevelTransform != null) {
                return topLevelTransform.GetComponent<T>();
            }
            return default(T);
        }

        public T GetComponentInBoth<T>() where T : Component {
            T cmp;
            cmp = GetComponent<T>();
            if (cmp != null) {
                return cmp;
            }
            return GetTopLevelComponent<T>();
        }
    }
}
