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
        [SerializeField] private GameObject mainhandWeapon1;
        [SerializeField] private GameObject mainhandWeapon2;
        [SerializeField] private GameObject offhandWeapon1;
        [SerializeField] private GameObject offhandWeapon2;

        [SerializeField] private Transform mainhandWeaponPosition;
        [SerializeField] private Transform offhandWeaponPosition;
        [SerializeField] private Entity entity;
        [SerializeField] private Animator animator;
        [SerializeField] private AbilityRunner abilityRunner;

        private Weapon mainWeapon1;
        private Weapon mainWeapon2;
        private Weapon offWeapon1;
        private Weapon offWeapon2;

        private GameObject mainWeapon1Inst;
        private GameObject mainWeapon2Inst;
        private GameObject offWeapon1Inst;
        private GameObject offWeapon2Inst;

        public Weapon Mainhand { get; private set; }
        public Weapon Mainhand2 { get; private set; }
        public Weapon Offhand { get; private set; }
        public Weapon Offhand2 { get; private set; }

        public Entity Wielder { get { return entity; } }

        private void Start()
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
                    mainWeapon2.gameObject.SetActive(false);
                    Mainhand2 = mainWeapon2;
                }
                mainWeapon2.weaponWielder = this;
            }
            
            if(offhandWeapon1 != null) {
                offWeapon1Inst = Instantiate(offhandWeapon1);
                offWeapon1 = offWeapon1Inst.GetComponentInChildren<Weapon>();
                offWeapon1.Entity = entity;
                offWeapon1Inst.transform.position = offhandWeaponPosition.position;
                offWeapon1Inst.transform.rotation = offhandWeaponPosition.rotation; 
                offWeapon1Inst.transform.SetParent(offhandWeaponPosition);
                Offhand = offWeapon1;
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
                    offWeapon2.gameObject.SetActive(false);
                    Offhand2 = offWeapon2;
                }
            }
            if (Mainhand != null && Mainhand.DoubleHanded) {
                if (Offhand != null)
                {
                    Offhand.gameObject.SetActive(false);
                }
            }

            PlayDefaultWeaponAnimation();
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
                    w.gameObject.SetActive(false);
                    Mainhand.gameObject.SetActive(true);
                    PlayDefaultWeaponAnimation();
                }
            }
            if (Offhand != null)
            {
                if (Mainhand != null && Mainhand.DoubleHanded)
                {
                    Offhand.gameObject.SetActive(false);
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
                    Offhand.gameObject.SetActive(false);
                    Offhand = Offhand2;
                    Offhand2 = w;
                    if (Mainhand != null && !Mainhand.DoubleHanded) {
                        Offhand.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void PlayDefaultWeaponAnimation() {
            if(Mainhand != null && animator != null)
            {
                animator.speed = 1;
                if (abilityRunner != null && !abilityRunner.IsAnimating()) {
                    animator.CrossFade(Mainhand.Name + "_move", 0.1f, -1, 0.1f);
                }
            }
        }
    }
}
