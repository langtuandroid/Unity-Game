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
        private int move_id = -1;
        private AbilityRunner ar;
        protected override void OnApply()
        {
            move_id = entity.BlockMovement();
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
            entity.UnblockMovement(move_id);
            move_id = -1;
        }
    }
}
