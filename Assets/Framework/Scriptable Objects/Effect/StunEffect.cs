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
        private AbilityRunner abilityRunner;
        private MovementController moveControl;

        protected override void OnApply()
        {
            moveControl = entity.GetComponent<MovementController>(); 
            if (moveControl != null)
            {
                move_id = moveControl.BlockMovement();
            }
            
            abilityRunner = entity.GetComponent<AbilityRunner>();
            if (abilityRunner != null) {
                effector_id = abilityRunner.BlockAction();
            }
        }

        protected override void OnEffectOver()
        {
            if (effector_id != -1) {
                abilityRunner.UnblockAction(effector_id);
                effector_id = -1;
            }
            if(move_id != -1)
            {
                moveControl.UnblockMovement(move_id);
                move_id = -1;
            }
        }
    }
}
