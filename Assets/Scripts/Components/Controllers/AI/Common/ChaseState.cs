using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    private AIController aiController;
    private Mana manaComponent;
    private CombatComponent combatComponent;
    private Actionable actionComponent;

    private Actionable targetAction;
    private CombatComponent targetCombatComponent;

    private Entity chaseTarget;

    public override void InitializeFields(GameObject obj)
    {
        aiController = obj.GetComponent<AIController>();
        actionComponent = obj.GetComponent<Actionable>();
        manaComponent = actionComponent.GetActionComponent<Mana>();
        combatComponent = actionComponent.GetActionComponent<CombatComponent>();
    }

    public override void OnEnter()
    {
        chaseTarget = aiController.target;
        targetAction = chaseTarget.GetComponent<Actionable>();
        if (targetAction != null)
        {
            targetCombatComponent = targetAction.GetActionComponent<CombatComponent>();
        }
    }

    public override void OnExit()
    {
        targetCombatComponent = null;
        aiController.target = null;
    }

    public override Type Tick()
    {
        if (!aiController.target.gameObject.activeInHierarchy) {
            return typeof(WanderState);
        }
        aiController.ChaseTarget();
        if (aiController.InChaseRange()) {
            if (aiController.TargetInRange(combatComponent.attackRange.Value))
            {
                actionComponent.EnqueueAction<CircleAttack>();
            }
            if (IsVulnerableToMelee()) {
                actionComponent.EnqueueAction<Guard>();
            }
            return null;
        }
        return typeof(WanderState);
    }

    private bool IsVulnerableToMelee() {
        if (targetAction == null) {
            return false;
        }
        return targetAction.IsInstanceReady<CircleAttack>() && aiController.TargetInRange(targetCombatComponent.attackRange.Value);
    }
}
