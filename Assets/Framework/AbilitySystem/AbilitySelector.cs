using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility;
using System;

namespace LobsterFramework.AbilitySystem
{
    [Serializable]
    public class AbilitySelector : SerializableType<Ability>
    {
        [SerializeField] private string configName;

        public string ConfigName { get { return configName; } }
    }
}
