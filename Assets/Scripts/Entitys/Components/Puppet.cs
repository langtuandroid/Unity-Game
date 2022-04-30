using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppet : Entity.EntityComponent
{
    /* Entities who carries this component operates on behalf of their masters
     * 
     */
    private Entity master;

    public Puppet(Entity e, Entity m) : base(e) { 
        master = m;
    }
}
