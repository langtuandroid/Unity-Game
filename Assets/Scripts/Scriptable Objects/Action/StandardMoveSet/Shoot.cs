using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ActionInstance(typeof(Shoot))]
[RequireActionComponent(typeof(Shoot), typeof(CombatComponent), typeof(Mana))]
public class Shoot : ActionInstance
{
    [SerializeField] private EntityGroup[] targetGroups;
    [SerializeField] private EntityGroup[] ignoreGroups;
    [SerializeField] private VarString bulletPoolTag;
    [SerializeField] private RefFloat manaCost;
    [SerializeField] private RefFloat bulletSpeed;
    [SerializeField] private RefInt piercePower;
    [SerializeField] private RefFloat travelDuration;

    private HashSet<Entity> targets = new();
    private HashSet<Entity> ignoreTargets = new();
    private Mana manaComponent;

    

    public override void Initialize()
    {
        foreach (EntityGroup group in targetGroups)
        {
            group.OnEntityAdded.AddListener((Entity entity) => { targets.Add(entity); });
        }

        foreach (EntityGroup group in ignoreGroups)
        {
            group.OnEntityAdded.AddListener((Entity entity) => { ignoreTargets.Add(entity); });
        }

        manaComponent = actionComponent.GetActionComponent<Mana>();
    }

    public override bool ConditionSatisfied()
    {
        return manaComponent.AvailableMana >= manaCost.Value;
    }

    protected override void OnEnqueue()
    {
        manaComponent.ReserveMana(manaCost.Value);
    }

    protected override void ExecuteBody()
    {
        CombatComponent combatComponent = actionComponent.GetActionComponent<CombatComponent>();
        int firePower = combatComponent.attackDamage;

        Transform transform = actionComponent.transform;
        GameObject obj = ObjectPool.Instance.GetObject(bulletPoolTag.Value, transform.position, transform.rotation);
        Bullet bullet = obj.GetComponent<Bullet>();
        if (bullet == null)
        {
            Debug.LogError("The prefab used under the tag: " + bulletPoolTag.Value + " is not a valid bullet prefab.");
            obj.SetActive(false);
            return;
        }

        Transform bulletTransform = obj.transform;
        bullet.Initialize(targets, ignoreTargets, travelDuration.Value, firePower, piercePower.Value);
        Rigidbody2D body = bullet.GetComponent<Rigidbody2D>();
        body.velocity = bulletTransform.up.normalized * bulletSpeed.Value;
    }
}
