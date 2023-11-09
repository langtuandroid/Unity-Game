using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LobsterFramework.Utility
{
    /// <summary>
    /// Stats that can buffered, meaning it can be affected by multiple effectors. <br/>
    /// i.e The player may be unable to act for some time due to multiple sources of negative effects, <br/>
    /// and the flag setting (buffered stat) that governs this player state will remain unchanged if not <br/> 
    /// all of these effects (effectors) are removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BufferedValue<T> where T : IEquatable<T>
    {
        private IdDistributor distributor = new();
        protected Dictionary<int, T> stats = new();
        protected T baseValue;
        private T value;
        internal Action onEffectorCleared;
        internal Action<T> onValueChanged;

        /// <summary>
        ///
        /// </summary>
        /// <param name="baseValue"> The base value when no effectors are present </param>
        public BufferedValue(T baseValue) { this.baseValue = baseValue; }

        /// <summary>
        /// Return thec cached buffered value
        /// </summary>
        public T Value { 
            get { return stats.Count == 0 ? baseValue : value; }
        }

        /// <summary>
        /// Determines if the effector can be added
        /// </summary>
        /// <param name="obj">The value of the effector to be examined</param>
        /// <returns>true if can be added, otherwise false</returns>
        public virtual bool Compatible(T obj) { return true; }

        internal int AddEffector(T obj) {
            if (!Compatible(obj)) {
                return -1;
            }
            int id = distributor.GetID();
            stats.Add(id, obj);
            T prevValue = value;
            value = ComputeValue();
            if (!prevValue.Equals(value)) {
                onValueChanged?.Invoke(value);
            }
            return id;
        }
        internal bool RemoveEffector(int id) {
            if (stats.Remove(id)) {
                T prevValue = value;
                value = ComputeValue();
                if (!prevValue.Equals(value))
                {
                    onValueChanged?.Invoke(value);
                }
                
                distributor.RecycleID(id);
                return true;
            }
            return false;
        }

        internal bool SetEffector(int id, T obj)
        {
            if (!Compatible(obj)) {
                return false;
            }
            if (stats.ContainsKey(id)) {
                stats[id] = obj;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove all effectors
        /// </summary>
        public void ClearEffectors() {
            foreach (int id in stats.Keys) {
                distributor.RecycleID(id);
            }
            stats.Clear();
            onEffectorCleared?.Invoke();
        }

        /// <summary>
        /// Returns a BufferedValueAccessor that manages setting and removing the effector
        /// </summary>
        /// <returns> A BufferedValueAccessor that manages setting and removing the effector </returns>
        public BufferedValueAccessor<T> GetAccessor() {
            return new(this);
        }

        /// <summary>
        /// The number of currently active effectors
        /// </summary>
        public int EffectorCount { get { return stats.Count; } }

        /// <summary>
        /// Compute the value taking all effectors into account
        /// </summary>
        /// <returns></returns>
        protected abstract T ComputeValue();
    }

    public class BufferedValueAccessor<T> where T : IEquatable<T> {
        private BufferedValue<T> stat;
        private int effectorID = -1;

        internal BufferedValueAccessor(BufferedValue<T> stat)
        {
            this.stat = stat;
            stat.onEffectorCleared += Release;
        }

        ~BufferedValueAccessor() {
            if (stat != null) {
                stat.onEffectorCleared -= Release;
            }
        }

        /// <summary>
        /// Acquire an ID from BufferedValue by setting a effector of specified value
        /// </summary>
        /// <param name="value">The value of the effector to be added</param>
        public void Acquire(T value) {
            if (effectorID == -1)
            {
                effectorID = stat.AddEffector(value);
            }
            else {
                Debug.LogWarning("Effector must be released before acquiring new ones.");
            }
        }

        /// <summary>
        /// Remove the effector from BufferedValue and release the ID to be used by others
        /// </summary>
        public void Release() {
            if (effectorID != -1)
            {
                stat.RemoveEffector(effectorID);
                effectorID = -1;
            }
        }

        /// <summary>
        /// Change the value of the effector added, effector value must be compatible
        /// </summary>
        /// <param name="value"> The value to change to </param>
        /// <returns> true if the effector value has been changed, otherwise false </returns>
        public bool Set(T value) {
            if (effectorID != -1)
            {
                return stat.SetEffector(effectorID, value);
            }
            else {
                Debug.LogWarning("Attempting to set effector value without acquiring!");
            }
            return false;
        }
    }

    /// <summary>
    /// Value is the sum of all effectors
    /// </summary>
    public class IntSum : BufferedValue<int>
    {
        public IntSum(int value) : base(value)
        {
        }

        protected override int ComputeValue()
        {
            return stats.Sum(pair => pair.Value) ;
        }
    }

    /// <summary>
    /// Value is the sum of all effectors
    /// </summary>
    public class FloatSum : BufferedValue<float>
    {
        private bool addNonNegative;
        private bool nonNegative;
        public FloatSum(int value, bool nonNegative=false, bool addNegative=true) : base(value)
        {
            this.nonNegative = nonNegative;
            this.addNonNegative = addNegative;
        }

        public override bool Compatible(float obj)
        {
            if (!addNonNegative && obj < 0) {
                return false;
            }
            return true;
        }

        protected override float ComputeValue()
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
    public class FloatProduct : BufferedValue<float>
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

        protected override float ComputeValue()
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
    public class BaseOr : BufferedValue<bool> {
        public BaseOr(bool value) : base(value)
        {
        }

        protected override bool ComputeValue()
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
    public class BaseAnd : BufferedValue<bool>
    {
        public BaseAnd(bool value) : base(value)
        {
        }

        protected override bool ComputeValue()
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
