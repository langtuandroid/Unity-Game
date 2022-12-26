using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Simple Item")]
public class SimpleItem : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private int itemLimit;
    [SerializeField] private Sprite icon;
    [TextArea]
    [SerializeField] private string description;

    public string ItemName { get { return itemName; } }

    public int ItemLimit { get { return itemLimit; } }

    public Sprite Icon { get { return icon; } }

    public string Description { get { return description; } }

    public virtual void Initialize(GameObject obj) { }
}
