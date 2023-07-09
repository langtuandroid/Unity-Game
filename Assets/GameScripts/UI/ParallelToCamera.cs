using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace GameScripts.UI
{
    public class ParallelToCamera : MonoBehaviour
    {
        private Transform cameraTransform;
        private new Transform transform;
        private void Start()
        {
            transform = GetComponent<Transform>();
            cameraTransform = Camera.main.GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation = cameraTransform.rotation;
        }
    }
}
