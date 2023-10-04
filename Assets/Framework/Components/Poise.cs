using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.Utility;

namespace LobsterFramework
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
        public readonly BaseOr hyperarmor = new(false);

        private Entity entity;
        private float poise;
        private float recoverTime;
        private float regenTime;

        public bool PoiseBroken { get; private set; }
        public float MaxPoise { get { return maxPoise; } }
        public float PoiseValue { get { return poise; } }
        public bool HyperArmored { get { return hyperarmor.Value; } }

        // Start is called before the first frame update
        void Start()
        {
            entity = GetComponent<Entity>();
            entity.onDamaged += OnDamageTaken;
            poise = maxPoise;
            hyperarmor.onValueChanged += OnHyperArmorStatusChanged;
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

        private void OnHyperArmorStatusChanged(bool hyperArmored) { 
            if (hyperArmored && PoiseBroken) {
                PoiseRecover();
            }
        }

        private void OnEnable()
        {
            hyperarmor.onValueChanged += OnHyperArmorStatusChanged;
        }

        private void OnDisable()
        {
            hyperarmor.onValueChanged -= OnHyperArmorStatusChanged;
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
