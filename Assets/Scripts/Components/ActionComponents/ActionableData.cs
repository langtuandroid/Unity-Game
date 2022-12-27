using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName ="Action/ActionableData")]
public class ActionableData : ScriptableObject
{
    public int identifier;
    public TypeActionComponentDictionary components = new();
    public TypeActionInstanceDictionary allActions = new();
    public Dictionary<string, ActionInstance> availableActions = new();

    public void Initialize(Actionable actionable) {
        availableActions.Clear();
        foreach (ActionInstance ai in allActions.Values) {
            if (ai.ComponentCheck(actionable))
            {
                ai.actionComponent = actionable;
                ai.Initialize();
                availableActions[ai.GetType().ToString()] = ai;
            }
        }
    }

    public void SaveContentsAsAsset() {
        if (AssetDatabase.Contains(this)) {
            foreach (ActionComponent cmp in components.Values) {
                AssetDatabase.AddObjectToAsset(cmp, this);
            }
            foreach (ActionInstance ai in allActions.Values) {
                AssetDatabase.AddObjectToAsset(ai, this);
            }
        }
    }

    public void CopyActionAsset() {
        List<ActionInstance> ais = new();
        foreach (var kwp in allActions)
        {
            ActionInstance ai = Instantiate(kwp.Value);
            ais.Add(ai);
        }
        foreach (var ai in ais) {
            allActions[ai.GetType().ToString()] = ai;
        }

        List<ActionComponent> acs = new();
        foreach (var kwp in components)
        {
            ActionComponent ac = Instantiate(kwp.Value);
            acs.Add(ac);
        }
        foreach (var ac in acs)
        {
            components[ac.GetType().ToString()] = ac;
        }
    }

    private T GetActionInstance<T>() where T : ActionInstance
    {
        string type = typeof(T).ToString();
        if (allActions.ContainsKey(type))
        {
            return (T)allActions[type];
        }
        return default;
    }

    private T GetActionComponent<T>() where T : ActionComponent {
        string type = typeof(T).ToString();
        if (components.ContainsKey(type))
        {
            return (T)components[type];
        }
        return default;
    }

    public bool ActionComponentCheck<T>() where T : ActionInstance
    {
        Type type = typeof(T);
        bool f1 = true;

        if (RequireActionComponentAttribute.requirement.ContainsKey(type))
        {
            HashSet<Type> ts = RequireActionComponentAttribute.requirement[type];

            List<Type> lst1 = new List<Type>();
            foreach (Type t in ts)
            {
                if (!components.ContainsKey(t.ToString()))
                {
                    lst1.Add(t);
                    f1 = false;
                }
            }
            if (f1)
            {
                return true;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Missing ActionComponent: ");
            foreach (Type t in lst1)
            {
                sb.Append(t.Name);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            Debug.LogError(sb.ToString());
            return false;
        }

        return true;
    }

    public bool AddActionComponent<T>() where T : ActionComponent
    {
        string str = typeof(T).ToString();
        if (components.ContainsKey(str))
        {
            return false;
        }
        T instance = CreateInstance<T>();
        components[str] = instance;
        if (AssetDatabase.Contains(this)) {
            AssetDatabase.AddObjectToAsset(instance, this);
        }
        return true;
    }

    public bool AddActionInstance<T>() where T : ActionInstance
    {
        if (GetActionInstance<T>() != default)
        {
            return false;
        }
        
        if (ActionComponentCheck<T>()) {
            T ai = CreateInstance<T>();
            allActions.Add(typeof(T).ToString(), ai);
            if (AssetDatabase.Contains(this))
            {
                AssetDatabase.AddObjectToAsset(ai, this);
            }
            return true;
        }
        return false;
    }

    public bool RemoveActionInstance<T>() where T : ActionInstance
    {
        ActionInstance ai = GetActionInstance<T>();
        if (ai != null) {
            allActions.Remove(typeof(T).ToString());
            DestroyImmediate(ai, true);
            return true;
        }
        return false;
    }

    public bool RemoveActionComponent<T>() where T : ActionComponent
    {
        string str = typeof(T).ToString();
        if (components.ContainsKey(str))
        {
            if (RequireActionComponentAttribute.rev_requirement.ContainsKey(typeof(T)))
            {
                StringBuilder sb = new();
                sb.Append("Cannot remove action component: " + typeof(T).ToString() + ", since the following action instances requires it: ");
                bool flag = false;
                foreach (Type t in RequireActionComponentAttribute.rev_requirement[typeof(T)])
                {
                    if (allActions.ContainsKey(t.ToString()))
                    {
                        flag = true;
                        sb.Append(t.Name);
                        sb.Append(", ");
                    }
                }
                if (flag)
                {
                    sb.Remove(sb.Length - 2, 2);
                    Debug.LogError(sb.ToString());
                    return false;
                }
            }
        }
        T cmp = GetActionComponent<T>();
        if (cmp != null) {
            components.Remove(str);
            DestroyImmediate(cmp, true);
            return true;
        }
        return false;
    }
}
