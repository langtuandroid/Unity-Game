using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// Required for the character to use weapon abilities
    /// </summary>
    public class WeaponWielder : MonoBehaviour
    {
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private Transform weaponLocation;

        private Weapon weapon;
        private GameObject weaponPrefabInst;

        public Weapon Weapon { get { return weapon; } }

        private void Start()
        {
            weaponPrefabInst = Instantiate(weaponPrefab);
            weapon = weaponPrefabInst.GetComponent<Weapon>();
            Transform t = weaponPrefabInst.GetComponent<Transform>();
            t.position = weaponLocation.position;
            t.rotation = weaponLocation.rotation;
            t.SetParent(weaponLocation);
        }
    }
}
