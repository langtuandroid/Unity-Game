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

        private Weapon mainWeapon1;
        private Weapon mainWeapon2;
        private Weapon offWeapon1;
        private Weapon offWeapon2;

        private GameObject mainWeapon1Inst;
        private GameObject mainWeapon2Inst;
        private GameObject offWeapon1Inst;
        private GameObject offWeapon2Inst;

        public Weapon Mainhand { get; private set; }
        public Weapon Offhand { get; private set; }

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
                }
            }
            if (Mainhand != null && Mainhand.DoubleHanded) {
                if (Offhand != null)
                {
                    Offhand.gameObject.SetActive(false);
                }
            }

            PlayDefaulWeaponAnimation();
        }

        public void SwitchMainHand() {
            if (mainWeapon1 == Mainhand && Mainhand.state == WeaponState.Idle)
            {
                if (mainhandWeapon2 != null)
                {
                    Mainhand = mainWeapon2;
                    mainWeapon1.gameObject.SetActive(false);
                    mainWeapon2.gameObject.SetActive(true);
                    PlayDefaulWeaponAnimation();
                }
            }
            else if (mainWeapon2 == Mainhand && Mainhand.state == WeaponState.Idle) { 
                if(mainhandWeapon1 != null)
                {
                    Mainhand = mainWeapon1;
                    mainWeapon2.gameObject.SetActive(false);
                    mainWeapon1.gameObject.SetActive(true);
                    PlayDefaulWeaponAnimation();
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
            if (offWeapon1 == Offhand && Offhand.state == WeaponState.Idle)
            {
                if (offhandWeapon2 != null)
                {
                    Offhand.gameObject.SetActive(false);
                    Offhand = offWeapon2;
                    if (Mainhand != null && !Mainhand.DoubleHanded) {
                        Offhand.gameObject.SetActive(true);
                    }
                }
            }
            else if (offWeapon2 == Offhand && Offhand.state == WeaponState.Idle)
            {
                if (offhandWeapon1 != null)
                {
                    Offhand.gameObject.SetActive(false);
                    Offhand = offWeapon1;
                    if (Mainhand != null && !Mainhand.DoubleHanded)
                    {
                        Offhand.gameObject.SetActive(true);
                    }
                }
            }
        }

        internal void PlayDefaulWeaponAnimation() {
            if(Mainhand != null && animator != null)
            {
                animator.Play(Mainhand.Name + "_move", -1, 0);
            }
        }
    }
}
