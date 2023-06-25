using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem.Weapon
{
    /// <summary>
    /// Required for the character to use weapon abilities
    /// </summary>
    public class WeaponWielder : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;

        public Weapon Weapon { get { return weapon; } }
    }
}
