using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ActionInstance(typeof(Guard))]
public class Guard : ActionInstance
{
    private GameObject guardAnimation;
    protected override void ExecuteBody()
    {
        Entity defender = actionComponent.GetComponent<Entity>();
        if (defender == null) {
            Debug.Log("The object is missing entity component to complete the action!");
            return;
        }
        CombatComponent combat = actionComponent.GetActionComponent<CombatComponent>();
        if (combat == null) {
            Debug.Log("The object is missing the required action component to complete the action!");
            return;
        }
        if (defender.GetIncomingDamage() > 0)
        {
            Debug.Log("Blocked: " + combat.defense);
        }
        defender.SetIncomingDamage(defender.GetIncomingDamage() - combat.defense);
        if (guardAnimation == null)
        {
            GameObject gameObject = defender.gameObject;
            Transform transform = gameObject.transform;
            Color color = Color.blue;
            color.a = 0.5f;
            guardAnimation = ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(2, 2, 1), color, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        }
    }

    protected override void Terminate()
    {
        Object.Destroy(guardAnimation);
        guardAnimation = null;
    }
}
