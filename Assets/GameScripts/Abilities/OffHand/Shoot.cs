using UnityEngine;
using LobsterFramework.Pool;
using LobsterFramework.AbilitySystem;
using LobsterFramework;
using System.Collections.Generic;
using LobsterFramework.Utility;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [OffhandWeaponAbility]
    [RequireWeaponStat(typeof(Firearm))]
    [AddWeaponArtMenu(false, WeaponType.Firearm)]
    public class Shoot : WeaponAbility
    {
        private Firearm firearm;
        private MovementController moveControl;
        private AnimationClip clip;
        private Weapon currentWeapon;
        private BufferedValueAccessor<float> moveModifier;
        private BufferedValueAccessor<float> rotateModifier;

        protected override void Init()
        {
            clip = WeaponWielder.GetAbilityClip(GetType(), WeaponType.Firearm);
            moveControl = abilityRunner.GetComponentInBoth<MovementController>();
            moveModifier = moveControl.moveSpeedModifier.GetAccessor();
            rotateModifier = moveControl.moveSpeedModifier.GetAccessor();
        }

        protected override bool WConditionSatisfied(AbilityConfig config) 
        {
            // Todo: Add bullet check
            return true;
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            // Todo: Add bullet Consumption
            currentWeapon = WeaponWielder.Offhand;
            firearm = currentWeapon.GetWeaponStat<Firearm>();
            ShootConfig config = (ShootConfig)CurrentConfig;
            config.signaled = false;
            abilityRunner.StartAnimation(this, CurrentConfigName, clip, currentWeapon.AttackSpeed);
            moveModifier.Acquire(config.moveSpeedModifier);
            rotateModifier.Acquire(config.rotateSpeedModifier);
            
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe)
        {
            ShootConfig config = (ShootConfig)CurrentConfig;
            while (!config.signaled) {
                yield return null;
            }
            Transform weaponTransform = currentWeapon.transform;
            GameObject obj = ObjectPool.GetObject(firearm.Bullet, weaponTransform.position, weaponTransform.rotation);
            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("The prefab used under the tag: " + firearm.Bullet + " is not a valid bullet prefab.");
                obj.SetActive(false);
                yield return CoroutineOption.Exit; 
            }
            ObjectPool.GetObject(firearm.MuzzleVFX, currentWeapon.Head.position, currentWeapon.transform.rotation, currentWeapon.transform);

            Transform bulletTransform = bullet.transform;
            bullet.Initialize(config.targetSetting, firearm.TravelTime, WeaponWielder.Offhand.Entity, firearm.Power, firearm.Penetration, firearm.Weight);
            Rigidbody2D body = bullet.GetComponent<Rigidbody2D>();
            body.velocity = bulletTransform.up.normalized * firearm.Speed;
        }

        protected override void OnCoroutineFinish()
        {
            ShootConfig config = (ShootConfig)CurrentConfig;
            moveModifier.Release();
            rotateModifier.Release();
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }

        protected override void Signal(AnimationEvent animationEvent)
        {
            // Ignore non-animation signals
            if (animationEvent != null) {
                ShootConfig config = (ShootConfig)CurrentConfig;
                config.signaled = true;
            }
        }

        public class ShootConfig : AbilityCoroutineConfig
        {
            public TargetSetting targetSetting;
            [Range(0, 1)] public float moveSpeedModifier;
            [Range(0, 1)] public float rotateSpeedModifier;

            [HideInInspector] public bool signaled;
        }

        public class ShootPipe : AbilityPipe { }
    }
}
