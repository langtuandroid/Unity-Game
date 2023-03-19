using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.QuestSystem
{
    public class HealthLogOperation : Operation
    {
        [SerializeField] private Entity entity;

        public override void Begin()
        {
            Debug.Log("Selected Entity Health: " + entity.Health);
        }
    }
}
