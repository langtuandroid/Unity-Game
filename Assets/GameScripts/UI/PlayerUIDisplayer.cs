using UnityEngine;
using UnityEngine.UI;
using LobsterFramework.EntitySystem;
using LobsterFramework.AbilitySystem;
using GameScripts.Abilities;

namespace GameScripts.UI
{
    public class PlayerUIDisplayer : MonoBehaviour
    {
        public Entity player;
        public Slider healthSlider;
        public Slider manaSlider;
        public AbilityRunner actionComponent;
        private Mana manaComponent;



        // Start is called before the first frame update
        void Start()
        {
            manaComponent = actionComponent.GetAbilityStat<Mana>();
        }

        // Update is called once per frame
        void Update()
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = player.MaxHealth;
            healthSlider.value = player.Health;

            manaSlider.maxValue = player.MaxPosture;
            manaSlider.value = player.Posture;
        }
    }
}
