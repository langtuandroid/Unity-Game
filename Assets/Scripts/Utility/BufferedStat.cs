using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace LobsterFramework.Utility.BufferedStats
{
    /// <summary>
    /// Stats that can buffered, meaning it can be affected by multiple effectors. <br/>
    /// i.e The player may be unable to act for some time due to multiple sources of negative effects, <br/>
    /// and the flag setting (buffered stat) that governs this player state will remain unchanged if not <br/> 
    /// all of these effects (effectors) are removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BufferedStat<T>
    {
        private IdDistributor distributor = new();
        protected Dictionary<int, T> stats = new();
        protected T baseValue;
        private T value;

        public BufferedStat(T value) { baseValue = value; }

        public T Stat { 
            get { return stats.Count == 0 ? baseValue : value; }
        }

        public virtual bool Compatible(T obj) { return true; }

        public int AddEffector(T obj) {
            if (!Compatible(obj)) {
                return -1;
            }
            int id = distributor.GetID();
            stats.Add(id, obj);
            value = Value();
            return id;
        }
        public bool RemoveEffector(int id) {
            if (stats.Remove(id)) {
                value = Value();
                distributor.RecycleID(id);
                return true;
            }
            return false;
        }

        public bool SetEffector(int id, T obj)
        {
            if (stats.ContainsKey(id)) {
                stats[id] = obj;
                return true;
            }
            return false;
        }
        public void ClearEffectors() {
            foreach (int id in stats.Keys) {
                distributor.RecycleID(id);
            }
            stats.Clear();
        }

        protected abstract T Value();
    }

    /// <summary>
    /// Value is the sum of all effectors
    /// </summary>
    public class IntSum : BufferedStat<int>
    {
        public IntSum(int value) : base(value)
        {
        }

        protected override int Value()
        {
            return stats.Sum(pair => pair.Value) ;
        }
    }

    /// <summary>
    /// Value is the sum of all effectors
    /// </summary>
    public class FloatSum : BufferedStat<float>
    {
        private bool addNonNegative;
        private bool nonNegative;
        public FloatSum(int value, bool nonNegative=false, bool addNonNegative=true) : base(value)
        {
            this.nonNegative = nonNegative;
            this.addNonNegative = addNonNegative;
        }

        public override bool Compatible(float obj)
        {
            if (!addNonNegative && obj < 0) {
                return false;
            }
            return true;
        }

        protected override float Value()
        {
            float total = 0;
            foreach (var pair in stats) {
                total += pair.Value;
            }
            if (nonNegative && total < 0)
            {
                total = 0;
            }
            return total;
        }
    }

    /// <summary>
    /// Value is the product of all effectors
    /// </summary>
    public class FloatProduct : BufferedStat<float>
    {
        private bool nonNegative;

        public FloatProduct(float value, bool nonNegative=false) : base(value)
        {
            this.nonNegative = nonNegative;
        }

        public override bool Compatible(float obj)
        {
            if (nonNegative && obj < 0) {
                return false;
            }
            return true;
        }

        protected override float Value()
        {
            float value = baseValue;
            foreach (var pair in stats) { 
                value *= pair.Value;
            }
            return value;  
        }
    }

    /// <summary>
    /// Value is true if one effector is true, otherwise return base value
    /// </summary>
    public class BaseOr : BufferedStat<bool> {
        public BaseOr(bool value) : base(value)
        {
        }

        protected override bool Value()
        {
            foreach (var pair in stats) {
                if (pair.Value) {
                    return true;
                }
            }
            return baseValue;
        }
    }

    /// <summary>
    /// Value is true if all effectors are true, otherwise return base value
    /// </summary>
    public class BaseAnd : BufferedStat<bool>
    {
        public BaseAnd(bool value) : base(value)
        {
        }

        protected override bool Value()
        {
            foreach (var pair in stats)
            {
                if (!pair.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
