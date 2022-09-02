using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireActionComponent(typeof(Guard), typeof(CombatComponent))]
[ActionInstance(typeof(Guard))]
public class Guard : ActionInstance
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private RefFloat spriteAlpha;
    [SerializeField] private RefFloat guardRange;

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
        if (defender.IncomingDamage > 0)
        {
            Debug.Log("Blocked: " + combat.defense);
        }
        defender.IncomingDamage -= combat.defense;
        if (guardAnimation == null)
        {
            GameObject gameObject = defender.gameObject;
            Transform transform = gameObject.transform;
            Color color = Color.white;
            color.a = spriteAlpha.Value;
            float range = guardRange.Value;
            guardAnimation = ObjectGenerator.GenerateSprite(transform, sprite, transform.position, transform.rotation, new Vector3(range, range, 1), color, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Visual Effect"));
        }
    }

    protected override void Terminate()
    {
        Destroy(guardAnimation);
        guardAnimation = null;
    }
}
