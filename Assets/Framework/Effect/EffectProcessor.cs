using LobsterFramework.Effects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework
{
    public class EffectProcessor : SubLevelComponent
    {
        [SerializeField] public Dictionary<Type, Effect> activeEffects = new();

        // Update is called once per frame
        void Update()
        {
            List<Effect> removed = new();
            foreach (Effect effect in activeEffects.Values)
            {
                if (!effect.Update())
                {
                    removed.Add(effect);
                }
            }
            foreach (Effect effect in removed)
            {
                activeEffects.Remove(effect.GetType());
                Destroy(effect);
            }
        }

        /// <summary>
        /// Apply an effect to the entity
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffect(Effect effect)
        {
            Type t = effect.GetType();
            if (activeEffects.ContainsKey(t))
            {
                return;
            }
            Effect eft = Instantiate(effect);
            activeEffects.Add(t, eft);
            eft.ActivateEffect(this);
        }

        private void OnDisable()
        {
            foreach (Effect effect in activeEffects.Values)
            {
                effect.TerminateEffect();
                Destroy(effect);
            }
            activeEffects.Clear();
        }
    }
}
