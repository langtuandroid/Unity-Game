using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.AbilitySystem
{
    public class WeaponUtility : MonoBehaviour
    {
        private static WeaponUtility instance;
        [Range(0, 1)]
        [SerializeField] private float hdSharpness;
        [Range(0, 1)]
        [SerializeField] private float hdWeight;
        [Range(0, 1)]
        [SerializeField] private float pdSharpness;
        [Range(0, 1)]
        [SerializeField] private float pdWeight;

        public static float HDSharpness { get { return instance.hdSharpness; } }
        public static float HDWeight { get { return instance.hdWeight; } }
        public static float PDSharpness { get { return instance.pdSharpness; } }
        public static float PDWeight { get {  return instance.pdWeight; } }

        public static void WeaponDamage(Weapon weapon, Entity target, float hdModifier = 1, float pdModifier = 1) {
            float hd = weapon.Sharpness * HDSharpness + weapon.Weight * HDWeight;
            float pd = weapon.Sharpness * PDSharpness + weapon.Weight * PDWeight;
            hd = hd * hdModifier;
            pd = pd * pdModifier;
            target.Damage(hd, pd, weapon.Entity);
            MovementController moveControl = target.GetComponent<MovementController>();
            if (moveControl != null)
            {
                target.GetComponent<MovementController>().ApplyForce(target.transform.position - weapon.Entity.transform.position, pd);
            }
        }

        public static void WeaponDamage(Weapon weapon, Entity target, DamageModifier modifier, float numModifier = 1) {
            float hd = weapon.Sharpness * HDSharpness + weapon.Weight * HDWeight;
            float pd = weapon.Sharpness * PDSharpness + weapon.Weight * PDWeight;
            Damage damage = new() { health=hd, posture=pd, source=weapon.Entity};
            damage = modifier.ModifyDamage(damage) * numModifier;
            target.Damage(damage);
            MovementController moveControl = target.GetComponent<MovementController>();
            if (moveControl != null)
            {
                target.GetComponent<MovementController>().ApplyForce(target.transform.position - weapon.Entity.transform.position, damage.posture);
            }
        }

        public static void GuardDamage(Weapon weapon, Weapon guardingWeapon, float hdModifier = 1, float pdModifier = 1) {
            float hd = weapon.Sharpness * HDSharpness + weapon.Weight * HDWeight;
            float pd = weapon.Sharpness * PDSharpness + weapon.Weight * PDWeight;

            hd = (1 - guardingWeapon.HealthDamageReduction) * hd * hdModifier;
            pd = (1 - guardingWeapon.PostureDamageReduction) * pd * pdModifier;
            Entity target = guardingWeapon.Entity;
            target.Damage(hd, pd, weapon.Entity);
            MovementController moveControl = target.GetComponent<MovementController>();
            if (moveControl != null)
            {
                target.GetComponent<MovementController>().ApplyForce(target.transform.position - weapon.Entity.transform.position, pd);
            }
        }

        public static void GuardDamage(Weapon weapon, Weapon guardingWeapon, DamageModifier modifier, float numModifier=1)
        {
            float hd = weapon.Sharpness * HDSharpness + weapon.Weight * HDWeight;
            float pd = weapon.Sharpness * PDSharpness + weapon.Weight * PDWeight;

            hd = (1 - guardingWeapon.HealthDamageReduction) * hd;
            pd = (1 - guardingWeapon.PostureDamageReduction) * pd;
            Damage damage = new() { health = hd, posture = pd, source = weapon.Entity };
            damage = modifier.ModifyDamage(damage) * numModifier;

            Entity target = guardingWeapon.Entity;
            target.Damage(damage);
            MovementController moveControl = target.GetComponent<MovementController>();
            if (moveControl != null) {
                target.GetComponent<MovementController>().ApplyForce(target.transform.position - weapon.Entity.transform.position, damage.posture);
            }
        }

        public static Damage ComputeDamage(Weapon weapon, float hdModifier = 1, float pdModifier = 1) {
            float hd = weapon.Sharpness * HDSharpness + weapon.Weight * HDWeight;
            float pd = weapon.Sharpness * PDSharpness + weapon.Weight * PDWeight;
            hd = hd * hdModifier;
            pd = pd * pdModifier;
            return new Damage() { health=hd, posture=pd, source=weapon.Entity};
        }

        public static Damage ComputeGuardedDamage(Weapon weapon, Weapon guardingWeapon, float hdModifier = 1, float pdModifier = 1) {
            float hd = weapon.Sharpness * HDSharpness + weapon.Weight * HDWeight;
            float pd = weapon.Sharpness * PDSharpness + weapon.Weight * PDWeight;

            hd = (1 - guardingWeapon.HealthDamageReduction) * hd * hdModifier;
            pd = (1 - guardingWeapon.PostureDamageReduction) * pd * pdModifier;
            return new Damage() { health = hd, posture = pd, source = weapon.Entity };
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else {
                Destroy(gameObject);
            }
        }
    }
}
