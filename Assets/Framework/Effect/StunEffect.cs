using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Utility;

namespace LobsterFramework.Effects
{
    [CreateAssetMenu(menuName = "Effect/Stun")]
    public class StunEffect : Effect
    {
        private BufferedValueAccessor<bool> abilityLock;
        private BufferedValueAccessor<bool> moveLock;
        private AbilityRunner abilityRunner;
        private MovementController moveControl;

        protected override void OnApply()
        {
            moveControl = processor.GetComponentInBoth<MovementController>(); 
            if (moveControl != null)
            {
                moveLock = moveControl.movementLock.GetAccessor();
                moveLock.Acquire(true);
            }
            
            abilityRunner = processor.GetComponentInBoth<AbilityRunner>();
            if (abilityRunner != null) {
                abilityLock = abilityRunner.actionLock.GetAccessor();
                abilityLock.Acquire(true);
            }
        }

        protected override void OnEffectOver()
        {
            if (moveControl != null)
            {
                moveLock.Release();
            }

            if (abilityRunner != null)
            {
                abilityLock.Release();
            }
        }
    }
}
