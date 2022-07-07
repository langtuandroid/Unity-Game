using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Conditions/CompleteQuest")]
public class CompleteQuest : Condition
{
    [SerializeField] string name;
    private Entity entity;

    public override bool Eval()
    {
        if (entity == null) {
            entity = GameObject.Find(name).GetComponent<Entity>();
        }
        
        return entity.GetHealth() <= 0;  
    }
}
