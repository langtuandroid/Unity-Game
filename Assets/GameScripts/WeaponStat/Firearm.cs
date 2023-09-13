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
        [SerializeField] private float weight;
        [SerializeField] private int pelletsPerShot;
        [SerializeField] private float spreadAngle; 
        [SerializeField] private int penetration;
        [SerializeField] private float speed;
        [SerializeField] private float travelTime;
        [SerializeField] private VarString bulletPrefab;
        [SerializeField] private VarString muzzleVFX;

        public float Power { get { return power; } }
        public float Weight { get { return weight; } } 
        public int Penetration { get {  return penetration; } }
        public float Speed { get { return speed;} }
        public int PelletsPerShot { get { return pelletsPerShot; } }
        public float SpreadAngle { get {  return spreadAngle; } }
        public float TravelTime { get { return travelTime;} }
        public string Bullet { get { return bulletPrefab.Value; } }
        public string MuzzleVFX { get { return muzzleVFX.Value; } }
    }
}
