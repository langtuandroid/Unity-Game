using UnityEngine;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.Effects
{
    [CreateAssetMenu(menuName = "Effect/Silent Effect")]
    public class SilentEffect : Effect
    {
        private AbilityRunner ar;
        private int effect_id = -1;

        protected override void OnApply()
        {
            ar = processor.GetComponentInBoth<AbilityRunner>();
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
