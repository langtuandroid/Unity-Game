using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Interaction
{
    [CreateAssetMenu(menuName = "Item/Consumable/HealthPotion")]
    public class HealthPotion : Item, IConsumable
    {
        [SerializeField] private int regenAmount;
        public void Consume(Inventory inventory)
        {
            inventory.Entity.Heal(new Damage() { health=regenAmount });
        }
    }
}
