using LobsterFramework.Interaction;
using LobsterFramework.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace LobsterFramework
{
    public class SerializableArray<T> : IList<T>, IEnumerable<T>, ICollection<T>
    {
        [SerializeField] protected T[] items;

        public T this[int index] { get => ((IList<T>)items)[index]; set => ((IList<T>)items)[index] = value; }

        public int Count => ((ICollection<T>)items).Count;

        public bool IsReadOnly => ((ICollection<T>)items).IsReadOnly;

        public void Add(T item)
        {
            ((ICollection<T>)items).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)items).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)items).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)this.items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)items).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)items).Insert(index, item);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)items).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)items).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }


    [Serializable]
    public class AnimationClipArray : SerializableArray<AnimationClip> { 
        public static implicit operator AnimationClip[](AnimationClipArray array) { if (array == null) { return null; } return array.items; }
        public static implicit operator AnimationClipArray(AnimationClip[] array) {
            if (array == null) { return null; }
            AnimationClipArray obj = new();
            obj.items = array; 
            return obj; 
        } 
    }
}
