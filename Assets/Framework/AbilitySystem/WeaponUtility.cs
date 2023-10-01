using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    public class WeaponUtility : MonoBehaviour
    {
        private static WeaponUtility instance;
        [Range(0, 1)]
        [Tooltip("The weight factor of weapon sharpness for health damage")]
        [SerializeField] private float hdSharpness;
        [Range(0, 1)]
        [Tooltip("The weight factor of weapon weight for health damage")]
        [SerializeField] private float hdWeight;
        [Range(0, 1)]
        [Tooltip("The weight factor of weapon sharpness for posture damage")]
        [SerializeField] private float pdSharpness;
        [Range(0, 1)]
        [Tooltip("The weight factor of weapon weight for posture damage")]
        [SerializeField] private float pdWeight;
        [Range(0, 1)]
        [Tooltip("The percentage of posture damage given to the attacker on weapon deflect.")]
        [SerializeField] private float postureDeflect;
        [Range(0, 1)]
        [Tooltip("The percentage of posture damage taken on weapon deflect.")]
        [SerializeField] private float pDamageOnDeflect;
        [Tooltip("The adjustment factor for the knockback force applied to entities on taking posture damage")]
        [SerializeField] private float knockbackAdjustment;

        public static float HDSharpness { get { return instance.hdSharpness; } }
        public static float HDWeight { get { return instance.hdWeight; } }
        public static float PDSharpness { get { return instance.pdSharpness; } }
        public static float PDWeight { get {  return instance.pdWeight; } }
        public static float PostureDeflect { get { return instance.postureDeflect; } }
        public static float PDamageOnDeflect { get { return instance.pDamageOnDeflect; } }
        public static float KnockbackAdjustment { get { return instance.knockbackAdjustment; } }

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

        public static Damage ComputeDamage(Weapon weapon, DamageModifier modifier=null, float numModifier=1) {
            float hd = weapon.Sharpness * HDSharpness + weapon.Weight * HDWeight;
            float pd = weapon.Sharpness * PDSharpness + weapon.Weight * PDWeight;
            Damage damage = new() { health = hd, posture = pd, source = weapon.Entity };
            if (modifier != null) {
                damage = modifier.ModifyDamage(damage);
            }
            return damage * numModifier;
        }

        public static Damage ComputeGuardDamage(Weapon weapon, Damage damage) {
            return new Damage {health=damage.health * (1 - weapon.HealthDamageReduction), posture = damage.posture * (1 - weapon.PostureDamageReduction)};
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
