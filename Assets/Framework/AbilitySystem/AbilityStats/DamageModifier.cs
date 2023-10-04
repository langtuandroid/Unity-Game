using LobsterFramework.Utility;
using System;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityStatMenu]
    public class DamageModifier : AbilityStat
    {
        [NonSerialized] public readonly FloatSum flatDamageModification = new(0, false, true);
        [NonSerialized] public readonly FloatProduct percentageDamageModifcation = new(1, true);
        [SerializeField]
        [ReadOnly] 
        private float flatDamage;
        [SerializeField]
        [ReadOnly]
        private float percentageDamage;

        public Damage ModifyDamage(Damage damage) {
            damage.health *= percentageDamageModifcation.Value;
            damage.posture *= percentageDamageModifcation.Value;

            damage.health += flatDamageModification.Value;
            damage.posture += flatDamageModification.Value;

            return damage;
        }

        public override void Update() {
            flatDamage = flatDamageModification.Value;
            percentageDamage = percentageDamageModifcation.Value;
        }
    }
}
