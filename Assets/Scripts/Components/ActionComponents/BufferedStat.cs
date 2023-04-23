using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LobsterFramework.Utility;

namespace LobsterFramework.AbilitySystem
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

        public BufferedStat(T value) { baseValue = value; }

        public T Stat { 
            get { return stats.Count == 0 ? baseValue : Value(); }
        }

        public int AddEffector(T obj) {
            int id = distributor.GetID();
            stats.Add(id, obj);
            return id;
        }
        public bool RemoveEffector(int id) { 
            return stats.Remove(id);
        }
        public void ClearEffectors() { 
            stats.Clear();
        }

        protected abstract T Value();
    }

    public class StackedInt : BufferedStat<int>
    {
        public StackedInt(int value) : base(value)
        {
        }

        protected override int Value()
        {
            return stats.Sum(pair => pair.Value) ;
        }
    }

    public class StackedBool : BufferedStat<bool> {
        public StackedBool(bool value) : base(value)
        {
        }

        protected override bool Value()
        {
            return stats.Sum(pair => pair.Value ? 1 : 0) > 0;
        }
    }
}
