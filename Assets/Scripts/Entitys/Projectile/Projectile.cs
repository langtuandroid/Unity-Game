using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : Entity
{
    // Start is called before the first frame update
    void Start()
    {
        customTags.Add(Setting.TAG_PROJECTILE);
    }

    public abstract void Construct(Dictionary<string, object> args);
}
