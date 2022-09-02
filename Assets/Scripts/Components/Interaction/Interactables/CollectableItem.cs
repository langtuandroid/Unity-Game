using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Interaction(interactable:typeof(CollectableItem), interactors: typeof(Inventory))]
[RequireComponent(typeof(SpriteRenderer))]
public class CollectableItem : InteractableObject
{
    [SerializeField] private RefBool destroyWhenDeplete;
    [SerializeField] private InventoryItem item;
    private SpriteRenderer spriteRenderer;

    private InteractionPrompt prompt = new() { Primary = "Collect"};

    private new void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemData.Icon;
        item.CorrectGeneralQuantity();
    }

    public InventoryItem Item { 
        get { return item; }  
        set {
            item = value;
            spriteRenderer.sprite = item.itemData.Icon;
            if (destroyWhenDeplete.Value)
            {
                if (item.Quantity == 0)
                {
                    Destroy(gameObject);
                }
            }
        } 
    }

    public override InteractionPrompt GetInteractionOptions(Type t)
    {
        return prompt;
    }
}
