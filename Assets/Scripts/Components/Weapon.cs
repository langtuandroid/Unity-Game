using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private float weight;
        [SerializeField] private float sharpness;
        [SerializeField] private GameObject wielder;
        new private Collider2D collider;

        // Start is called before the first frame update
        void Start()
        {
            collider.enabled = false;
        }

        public void Activate() { 
            collider.enabled = true;
        }

        public void Deactivate() { 
            collider.enabled=false;
        }
    }
}
