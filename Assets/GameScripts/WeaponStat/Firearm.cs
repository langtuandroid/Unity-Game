using LobsterFramework.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.Abilities
{
    [AddWeaponStatMenu]
    public class Firearm : WeaponStat
    {
        [SerializeField] private float power;
        [SerializeField] private int pelletsPerShot;
        [SerializeField] private float spreadAngle;
        [SerializeField] private int penetration;
        [SerializeField] private float speed;

        public float Power { get { return power; } }
        public int Penetration { get {  return penetration; } }
        public float Speed { get { return speed;} }
        public int PelletsPerShot { get { return pelletsPerShot; } }
        public float SpreadAngle { get {  return spreadAngle; } }
    }
}
