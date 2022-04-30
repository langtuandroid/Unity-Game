using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Entity.EntityComponent, IActionImplementor
{
    public Bullet(Entity e) : base(e){}

    public Dictionary<string, ActionInstance> AvailableActions()
    {
        throw new System.NotImplementedException();
    }

    public ActionInstance GetAction(string actionName)
    {
        throw new System.NotImplementedException();
    }

    public IActionImplementor GetIdentifier(string actionName)
    {
        throw new System.NotImplementedException();
    }

    public bool HasAction(string actionName)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
