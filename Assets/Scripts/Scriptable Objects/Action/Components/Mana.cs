using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AbilityStat(typeof(Mana))]
    public class Mana : AbilityStat
    {
        [SerializeField] private RefFloat maxMana;
        [Tooltip("Amount of mana regenerated per second.")]
        [SerializeField] private RefFloat manaRegen;

        [SerializeField] private float mana;
        private float reservedAmount;

        public float AvailableMana
        {
            get { return mana - reservedAmount; }
            set
            {
                mana = value;
                if (mana < 0)
                {
                    mana = 0;
                }
                else if (mana > maxMana.Value)
                {
                    mana = maxMana.Value;
                }
            }
        }

        public float MaxMana { get { return maxMana.Value; } }
        public float ManaRegen { get { return manaRegen.Value; } }

        public override void Initialize()
        {
            mana = maxMana.Value;
            reservedAmount = 0;
        }

        public override void Update()
        {
            mana -= reservedAmount;
            reservedAmount = 0;
            mana += manaRegen.Value * Time.deltaTime;
            float maxM = maxMana.Value;
            if (mana > maxM)
            {
                mana = maxM;
            }
        }

        public bool ReserveMana(float amount)
        {
            if (amount >= 0)
            {
                reservedAmount += amount;
            }
            return false;
        }
    }
}
