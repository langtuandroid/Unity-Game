using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Interaction
{
    public interface IConsumable
    {
        public void Consume(Inventory intentory);
    }
}
