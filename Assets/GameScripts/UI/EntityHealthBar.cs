using UnityEngine;
using UnityEngine.UI;
using LobsterFramework.EntitySystem;

namespace GameScripts.UI
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Transform target;
        
        public Entity entity;

        // Update is called once per frame
        void Update()
        {
            slider.value = entity.Health / entity.MaxHealth;
        }
    }
}
