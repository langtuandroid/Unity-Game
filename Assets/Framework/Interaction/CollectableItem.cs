using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Interaction
{
    [Interaction(interactors: typeof(Inventory))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CollectableItem : InteractableObject
    {
        [SerializeField] internal bool destroyWhenDeplete;
        [SerializeField] private InventoryItem item;

        private SpriteRenderer spriteRenderer;

        private InteractionPrompt prompt = new() { Primary = "Collect" };

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (item != null) {
                spriteRenderer.sprite = item.itemData.Icon;
            }
        }

        public InventoryItem Item
        {
            get { return item; }
            set
            {
                item = value;
                spriteRenderer.sprite = item.itemData.Icon; 
                if (destroyWhenDeplete)
                {
                    if (item.Quantity == 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        public void CheckStatus()
        {
            if (destroyWhenDeplete)
            {
                if (item.Quantity == 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        public override InteractionPrompt GetInteractionOptions(Type t)
        {
            return prompt;
        }
        public override void OnInteract(Interactor interactor, InteractionType interactType)
        {
            base.OnInteract(interactor, interactType);
        }
    }
}
