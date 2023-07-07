using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LobsterFramework.EntitySystem;

namespace GameScripts
{
    public class Healthbar : MonoBehaviour
    {
        // // Start is called before the first frame update
        // void Start()
        // {
            
        // }
        [SerializeField] private Slider slider;
        [SerializeField] private Camera camera;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 adjust;
        public Entity entity;

        // Update is called once per frame
        void Update()
        {
            slider.value = entity.Health/entity.MaxHealth;
            transform.rotation = camera.transform.rotation;
            transform.position = target.position + camera.transform.up;
        }
    }
}
