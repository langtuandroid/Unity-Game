using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.EntitySystem
{
    [CreateAssetMenu(menuName = "Effect/Silent Effect")]
    public class SilentEffect : Effect
    {
        private AbilityRunner ar;
        private int effect_id = -1;

        protected override void OnApply()
        {
            ar = entity.GetComponent<AbilityRunner>();
            if (ar != null) {
                effect_id = ar.BlockAction();
            }
        }

        protected override void OnEffectOver()
        {
            if (effect_id != -1) {
                ar.UnblockAction(effect_id);
                effect_id = -1;
            }
        }
    }
}
