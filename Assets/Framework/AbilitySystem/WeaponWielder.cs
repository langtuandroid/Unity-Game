using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using Animancer;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// Required for the character to use weapon abilities
    /// </summary>
    public class WeaponWielder : MonoBehaviour
    {
        [Header("Weapon Prefabs")]
        [SerializeField] private GameObject mainhandWeapon1;
        [SerializeField] private GameObject mainhandWeapon2;
        [SerializeField] private GameObject offhandWeapon1;
        [SerializeField] private GameObject offhandWeapon2;
        [SerializeField] private GameObject emptyHand;

        [Header("Weapon Position")]
        [SerializeField] private Transform mainhandWeaponPosition;
        [SerializeField] private Transform offhandWeaponPosition;

        [Header("Weapon Data")]
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private WeaponAnimationData animationData;
        private WeaponData data;
        private TypeWeaponStatDictionary weaponStats;

        [Header("Component Reference")]
        [SerializeField] private Entity entity;
        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private AbilityRunner abilityRunner;

        private Weapon mainWeapon1;
        private Weapon mainWeapon2;
        private Weapon offWeapon1;
        private Weapon offWeapon2;

        private GameObject mainWeapon1Inst;
        private GameObject mainWeapon2Inst;
        private GameObject offWeapon1Inst;
        private GameObject offWeapon2Inst;

        private Dictionary<Weapon, GameObject> objLookup = new();
        private bool weaponSwitching;

        public Weapon Mainhand { get; private set; }
        public Weapon Mainhand2 { get; private set; }
        public Weapon Offhand { get; private set; }
        public Weapon Offhand2 { get; private set; }

        public Entity Wielder { get { return entity; } }

        private void Awake()
        {
            if(mainhandWeapon1 != null) { 
                mainWeapon1Inst = Instantiate(mainhandWeapon1);
                mainWeapon1 = mainWeapon1Inst.GetComponentInChildren<Weapon>();
                mainWeapon1.Entity = entity;
                mainWeapon1Inst.transform.position = mainhandWeaponPosition.position;
                mainWeapon1Inst.transform.rotation = mainhandWeaponPosition.rotation;
                mainWeapon1Inst.transform.SetParent(mainhandWeaponPosition);
                Mainhand = mainWeapon1;
                mainWeapon1.weaponWielder = this;
                objLookup[mainWeapon1] = mainWeapon1Inst;
            }
            if(mainhandWeapon2 != null) {
                mainWeapon2Inst = Instantiate(mainhandWeapon2);
                mainWeapon2 = mainWeapon2Inst.GetComponentInChildren<Weapon>();
                mainWeapon2.Entity = entity; 
                mainWeapon2Inst.transform.position = mainhandWeaponPosition.position;
                mainWeapon2Inst.transform.rotation = mainhandWeaponPosition.rotation;
                mainWeapon2Inst.transform.SetParent(mainhandWeaponPosition);
                if (mainhandWeapon1 == null)
                {
                    Mainhand = mainWeapon2;
                }
                else { 
                    mainWeapon2Inst.SetActive(false);
                    Mainhand2 = mainWeapon2;
                }
                mainWeapon2.weaponWielder = this;
                objLookup[mainWeapon2] = mainWeapon2Inst;
            }
            if(offhandWeapon1 != null) {
                offWeapon1Inst = Instantiate(offhandWeapon1);
                offWeapon1 = offWeapon1Inst.GetComponentInChildren<Weapon>();
                offWeapon1.Entity = entity;
                offWeapon1Inst.transform.position = offhandWeaponPosition.position;
                offWeapon1Inst.transform.rotation = offhandWeaponPosition.rotation; 
                offWeapon1Inst.transform.SetParent(offhandWeaponPosition);
                Offhand = offWeapon1;
                objLookup[offWeapon1] = offWeapon1Inst;
            }
            if(offhandWeapon2 != null) {
                offWeapon2Inst = Instantiate(offhandWeapon2);
                offWeapon2 = offWeapon2Inst.GetComponentInChildren<Weapon>();
                offWeapon2.Entity = entity;
                offWeapon2Inst.transform.position = offhandWeaponPosition.position;
                offWeapon2Inst.transform.rotation = offhandWeaponPosition.rotation;
                offWeapon2Inst.transform.SetParent(offhandWeaponPosition);
                if (offhandWeapon1 == null)
                {
                    Offhand = offWeapon2;
                }
                else { 
                    offWeapon2Inst.SetActive(false);
                    Offhand2 = offWeapon2;
                }
                objLookup[offWeapon2] = offWeapon2Inst;
            }
            if (Mainhand != null && Mainhand.DoubleHanded) {
                if (Offhand != null)
                {
                    objLookup[Offhand].SetActive(false);
                }
            }

            if (weaponData != null) {
                data = weaponData.Clone();
                weaponStats = data.weaponStats;
            }
            weaponSwitching = false;
        }

        public T GetWeaponStat<T>() where T : WeaponStat {
            string key = typeof(T).AssemblyQualifiedName;
            if (weaponStats != null && weaponStats.ContainsKey(key)) {
                return (T)weaponStats[key];
            }
            return default;
        }

        public void SwitchMainHand() {
            if (Mainhand != null && Mainhand.state == WeaponState.Idle)
            {
                if (Mainhand2 != null)
                {
                    Mainhand.Pause();
                    Weapon w = Mainhand;
                    Mainhand = Mainhand2;
                    Mainhand2 = w;
                    objLookup[w].SetActive(false);
                    objLookup[Mainhand].SetActive(true);
                    if (abilityRunner.IsAnimating())
                    {
                        // Let the character state manager decide which animation to play
                        weaponSwitching = true;
                        abilityRunner.InterruptAbilityAnimation();
                    }
                    else { 
                        PlaySmoothAnimation();
                    }
                }
            }
            if (Offhand != null)
            {
                if (Mainhand != null && Mainhand.DoubleHanded)
                {
                    objLookup[Offhand].SetActive(false);
                }
                else {
                    Offhand.gameObject.SetActive(true);
                }
            }
        }

        public void SwitchOffHand()
        {
            if (Offhand != null && Offhand.state == WeaponState.Idle)
            {
                if (Offhand2 != null)
                {
                    Offhand.Pause();
                    Weapon w = Offhand;
                    objLookup[Offhand].SetActive(false);
                    Offhand = Offhand2;
                    Offhand2 = w;
                    if (Mainhand != null && !Mainhand.DoubleHanded) {
                        objLookup[Offhand].SetActive(true);
                    }
                }
            }
        }

        private void PlaySmoothAnimation() {
            if (Mainhand != null && animancer != null)
            {
                AnimationClip clip = animationData.GetMoveClip(Mainhand.WeaponType);
                animancer.Play(clip, 0.25f, FadeMode.FixedDuration).Speed = 1;
            }
        }

        private void PlayInstantAnimation() {
            if(Mainhand != null && animancer != null)
            {
                if (abilityRunner != null && !abilityRunner.IsAnimating()) {
                    animancer.Stop();
                    AnimationClip clip = animationData.GetMoveClip(Mainhand.WeaponType);
                    animancer.Play(clip).Speed = 1;
                }
            }
        }

        public void PlayWeaponAnimation(bool smooth=false) {
            if (weaponSwitching || smooth)
            {
                weaponSwitching = false;
                PlaySmoothAnimation();
            }
            else { 
                PlayInstantAnimation();
            }
        }

        public AnimationClip GetAbilityClip(Type abilityType, WeaponType weaponType) {
            return animationData.GetAbilityClip(Mainhand.WeaponType, abilityType);
        }

        public AnimationClip GetMainHandMoveClip(WeaponType weaponType)
        {
            return animationData.GetMoveClip(weaponType);
        }
    }
}
