using LobsterFramework.Utility.BufferedStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LobsterFramework.EntitySystem
{
    [RequireComponent (typeof(Entity))]
    public class Poise : MonoBehaviour
    {
        [SerializeField] private float maxPoise;
        [SerializeField] private float timeToRegenerate;
        [SerializeField] private float timeToRecover;

        /// <summary>
        /// Send true if poise broken, false on poise recover
        /// </summary>
        public UnityAction<bool> onPoiseStatusChange;

        private Entity entity;
        private float poise;
        private float recoverTime;
        private float regenTime;
        private BaseOr hyperarmor;

        public bool PoiseBroken { get; private set; }
        public float MaxPoise { get { return maxPoise; } }
        public float PoiseValue { get { return poise; } }
        public bool HyperArmored { get { return hyperarmor.Stat; } }

        // Start is called before the first frame update
        void Start()
        {
            entity = GetComponent<Entity>();
            entity.onDamaged += OnDamageTaken;
            poise = maxPoise;
            hyperarmor = new(false);
        }

        private void OnDamageTaken(Damage damage)
        {
            // Do nothing if already Poise Broken or PostureBroken
            if (HyperArmored || PoiseBroken || entity.PostureBroken) {
                return;
            }
            if (Time.time >= regenTime) {
                poise = maxPoise;
            }
            regenTime = Time.time + timeToRegenerate;
            poise -= damage.health;
            if (damage.type == DamageType.WeaponDeflect) {
                poise -= damage.posture;
            }
            if (poise < 0) {
                PoiseBreak();
            }
        }

        private void PoiseBreak() {
            onPoiseStatusChange(true);
            PoiseBroken = true;
            recoverTime = Time.time + timeToRecover;
        }

        private void PoiseRecover() {
            onPoiseStatusChange(false);
            PoiseBroken = false;
            poise = maxPoise;
        }

        public int HyperArmor() { 
            int effector = hyperarmor.AddEffector(true);
            if (PoiseBroken) {
                PoiseRecover();
            }
            return effector;
        }

        public bool DisArmor(int key) {
            return hyperarmor.RemoveEffector(key);
        }

        private void Update()
        {
            if (PoiseBroken && Time.time >= recoverTime) {
                PoiseRecover();
            }
            if (!PoiseBroken && Time.time >= regenTime) {
                poise = maxPoise;
            }
        }

        private void OnValidate()
        {
            if (maxPoise < 0) {
                maxPoise = 0;
            }
            if (recoverTime < 0) {
                recoverTime = 0;
            }
        }
    }
}
