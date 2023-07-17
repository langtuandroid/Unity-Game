using UnityEngine;
using UnityEngine.UI;
using LobsterFramework.EntitySystem;
using System;

namespace GameScripts.UI
{
    public class EntityHealthBar : MonoBehaviour
    {
        private CanvasGroup canvas;
        [SerializeField] private Slider slider = null;
        [SerializeField] private float transTime = 2;
        public Entity entity;
        float count = 0;
        [SerializeField] private float duration = 2 ;
        float prevHealth = 0;
        // Update is called once per frame
        void OnDamage(Damage hit)
        {
            count = 0;
            
        }
        private void OnEnable()
        {
            entity.onDamaged += OnDamage;
        }
        private void OnDisable()
        {
            entity.onDamaged -= OnDamage;
        }
        private void Start()
        {
            canvas = GetComponent<CanvasGroup>();
            canvas.alpha = 0;
        }
        int condition = 0;
        void Update()
        {
            if (condition == 1 && canvas.alpha !=1)
            {
                canvas.alpha += Time.deltaTime/transTime;
            }
            else if(condition == 2 && canvas.alpha != 0)
            {
                canvas.alpha -= Time.deltaTime/transTime;
            }
            else
            {
                condition = 0;
            }
            slider.value = entity.Health / entity.MaxHealth;
            if (count < duration)
            {
                count+=Time.deltaTime;
                condition = 1;
   
            }
            else
            {
                condition = 2;

            }

            prevHealth = entity.Health;


        }
    }
}
