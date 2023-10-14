using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    public class WeaponStat : ScriptableObject
    {
        public WeaponStat Clone() { 
            return Instantiate(this);
        }
    }
}
