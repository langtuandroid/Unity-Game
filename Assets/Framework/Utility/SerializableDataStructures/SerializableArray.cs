using LobsterFramework.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LobsterFramework
{
    public class SerializableArray<T> : IList<T>, IEnumerable<T>, ICollection<T>
    {
        [SerializeField] protected T[] array;

        public T this[int index] { get => ((IList<T>)array)[index]; set => ((IList<T>)array)[index] = value; }

        public int Count => ((ICollection<T>)array).Count;

        public bool IsReadOnly => ((ICollection<T>)array).IsReadOnly;

        public void Add(T item)
        {
            ((ICollection<T>)array).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)array).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)array).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)this.array).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)array).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)array).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)array).Insert(index, item);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)array).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)array).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }
    }


    [Serializable]
    public class AnimationClipArray : SerializableArray<AnimationClip> { 
        public static implicit operator AnimationClip[](AnimationClipArray array) { if (array == null) { return null; } return array.array; }
        public static implicit operator AnimationClipArray(AnimationClip[] array) {
            if (array == null) { return null; }
            AnimationClipArray obj = new(); 
            obj.array = array; 
            return obj; 
        } 
    }
}
