using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
[Interaction(interactable: typeof(Door), interactors: typeof(PlayerController))]
public class Door : InteractableObject
{
    [SerializeField] private RefBool isClosed;
    [SerializeField] private RefBool isLocked;
    [SerializeField] private Sprite closeSprite;
    [SerializeField] private Sprite openSprite;

    private bool locked;
    private bool closed;
    private Collider2D cld;
    private SpriteRenderer spriteRenderer;


    private readonly Dictionary<InteractionType, string> LockedInfo = new()
    {
        {InteractionType.Secondary, "Unlock" }
    };

    private readonly Dictionary<InteractionType, string> UnlockedClosedInfo = new()
    {
        { InteractionType.Primary, "Open"},
        { InteractionType.Secondary, "Lock" }
    };

    private readonly Dictionary<InteractionType, string> OpenedInfo = new()
    {
        { InteractionType.Primary, "Close" },
    };

    public bool IsClosed
    {
        get { return closed; }
        private set {
            closed = value;
            cld.enabled = value;
            if (closed)
            {
                spriteRenderer.sprite = closeSprite;
            }
            else { 
                spriteRenderer.sprite = openSprite;
            }
        }
    }

    public bool IsLocked { 
        get { return locked; }
        set { locked = value; }
    }

    new void Start()
    {
        base.Start(); // Do not delete this line
        cld = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsClosed = isClosed.Value;
        IsLocked = isLocked.Value;
    }

    public override string Interact(Interactor interactor, InteractionType interactType)
    {
        if (interactType == InteractionType.Primary) {
            if (closed)
            {
                if (!locked)
                {
                    IsClosed = false;
                }
            }
            else {
                IsClosed = true;
            }
        } else if (interactType == InteractionType.Secondary) {
            if (closed) {
                if (locked)
                {
                    IsLocked = false;
                }
                else {
                    IsLocked = true;   
                }
            }
        }
        return Setting.INTERACTION_OK;
    }

    public override Dictionary<InteractionType, string> GetInteractionType(Type t)
    {
        if (locked) {
            return LockedInfo;
        }
        if (closed) {
            return UnlockedClosedInfo;
        }
        return OpenedInfo;
    }
}
