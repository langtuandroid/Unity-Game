using Animancer;
using LobsterFramework;
using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.Abilities
{
    [ComponentRequired(typeof(WeaponWielder), typeof(Rigidbody2D), typeof(Collider2D))]
    [AddWeaponArtMenu(false, WeaponType.EmptyHand)]
    [AddAbilityMenu]
    public class OffhandGrab : AbilityCoroutine
    {
        private WeaponWielder weaponWielder;
        private Rigidbody2D rigidBody;
        private Collider2D collider;
        private Weapon currentWeapon;
        private AnimationClip clip;
        private Transform oldParentTransform = null;
        private Entity holdingEntity = null;
        private MovementController targetMoveControl;
        private CharacterStateManager targetStateManager = null;
        private int suppressKey = -1;


        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponentInBoth<WeaponWielder>();
            rigidBody = abilityRunner.GetComponentInBoth<Rigidbody2D>();
            clip = weaponWielder.GetAbilityClip(GetType(), WeaponType.EmptyHand);
            collider = abilityRunner.GetComponentInBoth<Collider2D>();
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            return RunningCount == 0 && weaponWielder.Offhand != null && weaponWielder.Offhand.WeaponType == WeaponType.EmptyHand;
        }


        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            OffhandGrabConfig config = (OffhandGrabConfig)CurrentConfig;
            config.signaled = false;
            currentWeapon = weaponWielder.Offhand;
            abilityRunner.StartAnimation(this, CurrentConfigName, clip);
            SubscribeWeaponEvent();
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe)
        {
            OffhandGrabConfig gc = (OffhandGrabConfig)CurrentConfig;
            yield return CoroutineOption.Wait(0);
            // Wait for signal to start attack
            while (!gc.signaled) {
                yield return null;
            }
            gc.signaled = false;
            currentWeapon.Action();

            // Wait for signal to retract arm
            while (!gc.signaled) {
                yield return null;
            }
            gc.signaled = false;
            currentWeapon.Pause();

            // If no entities are grabbed, terminate ability immediately
            if (holdingEntity == null) {
                yield return CoroutineOption.Exit;
            }

            // Wait for signal to throw
            while (!gc.signaled)
            {
                yield return null;
            }
            gc.signaled = false;

            holdingEntity.transform.parent = oldParentTransform;
            targetMoveControl.KinematicBody(false);
            targetMoveControl.SetVelocityImmediate(rigidBody.velocity);
            targetMoveControl.EnableCollider();
            targetMoveControl.ApplyForce(currentWeapon.transform.up, gc.throwStrength);
            holdingEntity.Damage(gc.healthDamage, gc.postureDamage, currentWeapon.Entity);

            // Wait for the end of suppression
            yield return CoroutineOption.Wait(gc.suppressTime);
            targetStateManager.Release(suppressKey);
            holdingEntity = null;
            // Wait for animation to finish
            while (!gc.signaled) { 
                yield return null;
            }
        }

        protected override void OnCoroutineFinish()
        {
            UnsubscribeWeaponEvent();
            if (holdingEntity != null) {
                holdingEntity.transform.parent = oldParentTransform;
                holdingEntity = null;
                targetStateManager.Release(suppressKey);
                targetMoveControl.KinematicBody(false);
                targetMoveControl.EnableCollider();
                targetMoveControl.SetVelocityImmediate(rigidBody.velocity);
            }
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }

        protected override void Signal(AnimationEvent animationEvent)
        {
            // Do nothing if the signal is not from animation
            if (animationEvent == null) {
                return;
            }
            OffhandGrabConfig gc = (OffhandGrabConfig)CurrentConfig;
            gc.signaled = true;
        }

        private void SubscribeWeaponEvent() { 
            currentWeapon.onEntityHit += OnEntityHit;
        }

        private void UnsubscribeWeaponEvent()
        {
            currentWeapon.onEntityHit -= OnEntityHit;
            currentWeapon.Pause();
        }

        private void OnEntityHit(Entity entity) {
            OffhandGrabConfig config = (OffhandGrabConfig)CurrentConfig;
            if (config.targetSetting.IsTarget(entity)) {
                currentWeapon.Pause();
                oldParentTransform = entity.transform.parent;
                holdingEntity = entity;
                targetStateManager = entity.GetComponent<CharacterStateManager>();
                suppressKey = targetStateManager.Suppress();
                entity.transform.parent = currentWeapon.transform;
                targetMoveControl = entity.GetComponent<MovementController>();
                targetMoveControl.DisableCollider();
                targetMoveControl.KinematicBody(true);
            }
        }

        public class OffhandGrabConfig : AbilityCoroutineConfig
        {
            public TargetSetting targetSetting;
            public float healthDamage;
            public float postureDamage;
            public float throwStrength;
            public float suppressTime;

            [HideInInspector] public bool signaled;

            protected override void Validate()
            {
                if (healthDamage < 0)
                {
                    healthDamage = 0;
                }
                if (postureDamage < 0)
                {
                    postureDamage = 0;
                }
                if (throwStrength < 0) {
                    throwStrength = 0;
                }
                if (suppressTime < 0) {
                    suppressTime = 0;
                }
            }
        }

        public class OffhandGrabPipe : AbilityPipe { }
    }
}
