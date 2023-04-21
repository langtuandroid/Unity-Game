using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.EntitySystem
{
    [CreateAssetMenu(menuName = "Effect/Stun")]
    public class StunEffect : Effect
    {
        private int effector_id = -1;
        private AbilityRunner ar;
        protected override void OnApply()
        {
            entity.BlockMovement(true);
            ar = entity.GetComponent<AbilityRunner>();
            if (ar != null) {
                effector_id = ar.BlockAction();
            }
        }

        protected override void OnEffectOver()
        {
            if (effector_id != -1) {
                ar.UnblockAction(effector_id);
                effector_id = -1;
            }
            entity.BlockMovement(false);
        }
    }
}
