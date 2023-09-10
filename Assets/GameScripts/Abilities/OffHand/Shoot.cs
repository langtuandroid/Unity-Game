using UnityEngine;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using LobsterFramework.AbilitySystem;
using LobsterFramework;
using System.Collections;
using System.Collections.Generic;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [OffhandWeaponAbility]
    [RequireWeaponStat(typeof(Firearm))]
    [AddWeaponArtMenu(false, WeaponType.FireArm)]
    public class Shoot : WeaponAbility
    {
        private Firearm firearm;
        private Entity attacker;

        protected override void Init()
        {
            attacker = abilityRunner.GetComponent<Entity>();
            firearm = WeaponWielder.Mainhand.GetWeaponStat<Firearm>();
        }

        protected override bool WConditionSatisfied(AbilityConfig config)
        {
            // Todo: Add bullet check
            return true;
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            // Todo: Add bullet Consumption
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe)
        {
            ShootConfig config = (ShootConfig)CurrentConfig;

            Transform transform = abilityRunner.transform;
            GameObject obj = ObjectPool.Instance.GetObject(firearm.Bullet, transform.position + Vector3.Scale(transform.lossyScale, transform.up), transform.rotation);
            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("The prefab used under the tag: " + firearm.Bullet + " is not a valid bullet prefab.");
                obj.SetActive(false);
                yield return CoroutineOption.Exit;
            }

            Transform bulletTransform = obj.transform;
            bullet.Initialize(config.targetSetting, firearm.TravelTime, attacker, firearm.Power, firearm.Penetration, firearm.Weight);
            Rigidbody2D body = bullet.GetComponent<Rigidbody2D>();
            body.velocity = bulletTransform.up.normalized * firearm.TravelTime;
        }

        protected override void OnCoroutineFinish()
        {
            
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }

        public class ShootConfig : AbilityConfig
        {
            public TargetSetting targetSetting;
        }

        public class ShootPipe : AbilityPipe { }
    }
}
