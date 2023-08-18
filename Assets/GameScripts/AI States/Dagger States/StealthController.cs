using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.AI
{
    public class StealthController : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private List<SpriteRenderer> parts;
        void Start()
        {
            foreach(SpriteRenderer parts in parts)
            {
                Color color = parts.color;
                color.a = 0.4f;
                parts.color = color;
            }
        }
        public void Changetrans(float value)
        {
            foreach (SpriteRenderer parts in parts)
            {
                Color color = parts.color;
                color.a = value;
                parts.color = color;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
