using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
[Interaction(interactable: typeof(SimpleDoor), interactors: typeof(GeneralInteractor))]
public class SimpleDoor : InteractableObject
{
    [SerializeField] private RefBool isClosed;
    [SerializeField] private Sprite closeSprite;
    [SerializeField] private Sprite openSprite;

    private bool closed;
    private Collider2D cld;
    private SpriteRenderer spriteRenderer;

    private readonly InteractionPrompt ClosedInfo = new() { Primary = "Open" };

    private readonly InteractionPrompt OpenedInfo = new() { Primary = "Close" };

    private readonly string responseOpen = "Door Opened";
    private readonly string responseClose = "Door Closed";

    public bool IsClosed
    {
        get { return closed; }
        private set
        {
            closed = value;
            cld.enabled = value;
            if (closed)
            {
                spriteRenderer.sprite = closeSprite;
            }
            else
            {
                spriteRenderer.sprite = openSprite;
            }
        }
    }

    new void Start()
    {
        base.Start(); // Do not delete this line
        cld = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsClosed = isClosed.Value;
    }

    public override void OnInteract(Interactor interactor, InteractionType interactType)
    {
        if (interactType == InteractionType.Primary)
        {
            if (closed)
            {
                IsClosed = false;
                if (interactionChannel != null) {
                    interactionChannel.RaiseEvent(responseOpen);
                }
            }
            else
            {
                IsClosed = true;
                if (interactionChannel != null)
                {
                    interactionChannel.RaiseEvent(responseClose);
                }
            }
        }
    }

    public override InteractionPrompt GetInteractionOptions(Type t)
    {
        if (closed)
        {
            return ClosedInfo;
        }
        return OpenedInfo;
    }
}
