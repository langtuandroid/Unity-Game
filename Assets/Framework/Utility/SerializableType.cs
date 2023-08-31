using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.Utility
{
    [Serializable]
    public class SerializableType<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private string qualifieldTypeName;
        private Type type;

        public Type Type
        {
            get
            {
                return type;
            }
            set {
                if (value == null || value == typeof(T) || value.IsSubclassOf(typeof(T))) {
                    type = value;
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (qualifieldTypeName != default) {
                Type = Type.GetType(qualifieldTypeName);
            }
        }

        public void OnBeforeSerialize()
        {
            if (type != null) {
                qualifieldTypeName = type.AssemblyQualifiedName;
            }
        }


    }
}
