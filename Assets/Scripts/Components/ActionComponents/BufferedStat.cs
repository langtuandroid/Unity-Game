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

        public abstract T Stat();
    }

    public class StackedInt : BufferedStat<int>
    {
        public override int Stat()
        {
            return stats.Count == 0 ? 0 : stats.Sum(pair => pair.Value) ;
        }
    }

    public class StackedBool : BufferedStat<bool> {
        public override bool Stat()
        {
            return (stats.Count > 0) && stats.Sum(pair => pair.Value ? 1 : 0) > 0;
        }
    }
}
