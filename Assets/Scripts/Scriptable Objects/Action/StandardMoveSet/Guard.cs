using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireActionComponent(typeof(Guard), typeof(CombatComponent))]
[ActionInstance(typeof(Guard))]
public class Guard : ActionInstance
{
    [SerializeField] private RefFloat duration;
    [SerializeField] private Sprite sprite;
    [SerializeField] private RefFloat spriteAlpha;
    [SerializeField] private VarString spritePoolTag;

    private float durationCounter;
    private bool actionStart;
    private TemporalObject guardAnimation;
    private CombatComponent combat;
    private Entity defender;

    public override void Initialize()
    {
        combat = actionComponent.GetActionComponent<CombatComponent>();
        defender = actionComponent.GetComponent<Entity>();
        if (defender == null)
        {
            Debug.LogError("The object is missing entity component to complete the action!");
        }
        if (combat == null)
        {
            Debug.LogError("The object is missing the required action component to complete the action!");
        }
        durationCounter = 0;
        actionStart = false;
    }

    protected override void OnEnqueue(){
        durationCounter = 0;
        actionStart = true;
    }

    protected override bool ExecuteBody()
    {
        if (!actionStart){
            durationCounter += Time.deltaTime;
            if (durationCounter > duration.Value) {
                return false;
            }
        }
        else { 
            actionStart = false;
        }

        if (defender.IncomingDamage > 0)
        {
            Debug.Log("Blocked: " + combat.defense.Value);
        }
        defender.IncomingDamage -= combat.defense.Value;
        if (guardAnimation == null){
            // Set up guard sprite
            GameObject g = ObjectPool.Instance.GetObject(spritePoolTag.Value, Vector3.zero, Quaternion.identity);
            Transform gTransform = g.transform;
            gTransform.SetParent(defender.transform);
            gTransform.localPosition = Vector3.zero;

            // Set Scale
            float range = combat.attackRange.Value;
            gTransform.localScale = Vector3.one;
            Vector3 scale = gTransform.lossyScale;
            gTransform.localScale = new Vector3(range / scale.x, range / scale.y, 1);

            TemporalObject tmp = g.GetComponent<TemporalObject>();
            tmp.ResetCounter();
            tmp.duration = -1;
            SpriteRenderer spriteRenderer = g.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            guardAnimation = tmp;
        }
        return true;
    }

    protected override void Terminate()
    {
        guardAnimation.DisableObject();
        guardAnimation = null;
    }
}
