using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AbilityStat(typeof(CombatStat))]
    public class CombatStat : AbilityStat
    {
        public RefInt attackDamage;
        public RefFloat attackRange;
        public RefInt defense;
    }
}
