using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Stun")]
public class StunEffect : Effect
{
    protected override void OnApply()
    {
        entity.BlockAction(true);
        entity.BlockMovement(true);
    }

    protected override void OnEffectOver()
    {
        entity.BlockAction(false);
        entity.BlockMovement(false);
    }
}
