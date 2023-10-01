using UnityEngine;
using UnityEngine.UI;
using LobsterFramework;
using LobsterFramework.AbilitySystem;
using GameScripts.Abilities;

namespace GameScripts.UI
{
    public class PlayerUIDisplayer : MonoBehaviour
    {
        public Entity player;
        public Slider healthSlider;
        public Slider postureSlider;
        public AbilityRunner abilityRunner;

        // Update is called once per frame
        void Update()
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = player.MaxHealth;
            healthSlider.value = player.Health;

            postureSlider.maxValue = player.MaxPosture;
            postureSlider.value = player.Posture;
        }
    }
}
