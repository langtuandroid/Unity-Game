using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Conditions/CompleteQuest")]
public class CompleteQuest : Condition
{
    [Header("ExposedReference<Entity>")]
    [SerializeField] string entityReference;
    [SerializeField] float health;
    private Entity entity;

    public override bool Eval()
    {
        if (entity == null) {
            ExposedReference<Entity> entityRef = new();
            entityRef.exposedName = entityReference;
            entity = entityRef.Resolve(ExposedPropertyTable.Instance);
        }
        return entity.GetHealth() <= health;  
    }
}
