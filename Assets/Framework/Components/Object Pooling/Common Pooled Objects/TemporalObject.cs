using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Pool
{
    public class TemporalObject : MonoBehaviour
    {
        public float duration;
        private float counter;

        public void ResetCounter()
        {
            counter = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (duration < 0)
            {
                return;
            }
            if (counter >= duration)
            {
                gameObject.SetActive(false);
                return;
            }
            counter += Time.deltaTime;
        }

        public void DisableObject()
        {
            gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            counter = 0;
        }
    }
}
