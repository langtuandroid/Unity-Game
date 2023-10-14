using LobsterFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [CreateAssetMenu(menuName = "Ability/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] public TypeWeaponStatDictionary weaponStats;

        public WeaponData Clone() {
            WeaponData data = CreateInstance<WeaponData>();
            data.weaponStats = new();
            foreach (KeyValuePair<string, WeaponStat> kwp in weaponStats) {
                data.weaponStats.Add(kwp.Key, kwp.Value.Clone());
            }
            return data;
        }

        /// <summary>
        /// Called by editor scritps, add WeaponStat of type T to the set of available WeaponStats if not already present, return the status of the operation. <br/>
        /// </summary>
        /// <typeparam name="T">Type of the WeaponStat to be added</typeparam>
        /// <returns>true if successfully added the WeaponStat, otherwise false</returns>
        internal bool AddWeaponStat<T>() where T : WeaponStat
        {
            string str = typeof(T).AssemblyQualifiedName;
            if (weaponStats.ContainsKey(str))
            {
                return false;
            }
            T instance = CreateInstance<T>();
            weaponStats[str] = instance;
            if (AssetDatabase.Contains(this))
            {
                AssetDatabase.AddObjectToAsset(instance, this);
                AssetDatabase.SaveAssets();
            }
            return true;
        }

        private T GetWeaponStat<T>() where T : WeaponStat
        {
            string key = typeof(T).AssemblyQualifiedName;
            if (weaponStats.ContainsKey(key)) {
                return (T)weaponStats[key];
            }
            return default;
        }

        internal bool RemoveWeaponStat<T>() where T : WeaponStat
        {
            string str = typeof(T).AssemblyQualifiedName;
            T cmp = GetWeaponStat<T>();
            if (cmp != null)
            {
                weaponStats.Remove(str);
                AssetDatabase.RemoveObjectFromAsset(cmp);
                DestroyImmediate(cmp, true);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }
    }
}
