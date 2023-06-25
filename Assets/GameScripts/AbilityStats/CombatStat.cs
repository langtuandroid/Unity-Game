using LobsterFramework.AbilitySystem;

namespace GameScripts.Abilities
{
    [AddAbilityStatMenu]
    public class CombatStat : AbilityStat
    {
        public RefInt attackDamage;
        public RefFloat attackRange;
        public RefInt defense;
    }
}
