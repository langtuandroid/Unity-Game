using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Interaction
{
    public class HealthPotion : Item, IConsumable
    {
        [SerializeField] private int RegenAmount;
        public void Consume(Inventory inventory)
        {
            
        }
    }
}
