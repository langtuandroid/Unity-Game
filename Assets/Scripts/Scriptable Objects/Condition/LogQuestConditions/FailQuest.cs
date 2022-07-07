using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Conditions/FailQuest")]
public class FailQuest : Condition
{
    [SerializeField] float timer;

    public override bool Eval()
    {
        timer -= Time.deltaTime;
        return timer <= 0;
    }
}
