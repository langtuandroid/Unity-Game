using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposedPropertyTable : MonoBehaviour, IExposedPropertyTable
{
    private static ExposedPropertyTable instance;

    [SerializeField] private List<Info> infos;
    [SerializeField] private Info addElement;

    [System.Serializable]
    public class Info {
        public Object objValue;
        public string identifier;

        public Info(string i, Object obj) {
            objValue = obj;
            identifier = i;
        }
    }

    public void ClearReferenceValue(PropertyName id)
    {
        infos.Clear();
    }

    public Object GetReferenceValue(PropertyName id, out bool idValid)
    {
        idValid = false;
        foreach (Info i in infos) {
            if (new PropertyName(i.identifier) == id) { 
                idValid = true;
                return i.objValue;
            }
        }
        return null;
    }

    public void SetReferenceValue(PropertyName id, Object value)
    {
    }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public bool ContainsValue(Object value) {
        foreach (Info item in infos) {
            if (value.Equals(item.objValue)) { 
                return true;
            }
        }
        return false;
    }

    public bool ContainsReference(string reference) {
        foreach (Info item in infos)
        {
            if (reference == item.identifier)
            {
                return true;
            }
        }
        return false;
    }

    public bool AddObjectReference(){
        Info info = addElement;
        if (info == null) {
            return false;   
        }
        if (ContainsValue(info.objValue)) {
            Debug.LogError("Object Already Exists inside table!");
            return false;   
        }
        if (ContainsReference(info.identifier)) {
            Debug.LogError("Identifier already in use!");
            return false;
        }
        infos.Add(new Info(info.identifier, info.objValue));
        return true;
    }

    public static ExposedPropertyTable Instance {
        get { return instance; }
    }
}
