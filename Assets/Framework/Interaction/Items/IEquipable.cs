using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Interaction
{
    public interface IEquipable
    {
        public void Equip(Inventory inventory);
    }
}
