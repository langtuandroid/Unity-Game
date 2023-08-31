using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using LobsterFramework.Utility.BufferedStats;
using System;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityStatMenu]
    public class DamageModifier : AbilityStat
    {
        [NonSerialized] public FloatSum flatDamageModification;
        [NonSerialized] public FloatProduct percentageDamageModifcation;
        public override void Initialize() {
            flatDamageModification = new(0, false, true);
            percentageDamageModifcation = new(1, true);
        }

        public Damage ModifyDamage(Damage damage) {
            damage.health *= percentageDamageModifcation.Stat;
            damage.posture *= percentageDamageModifcation.Stat;

            damage.health += flatDamageModification.Stat;
            damage.posture += flatDamageModification.Stat;

            return damage;
        }
    }
}
