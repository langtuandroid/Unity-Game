using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] private RefFloat interactRadius;
    [SerializeField] private InteractorListSO interactorList;
    private List<Transform> locations = new();
    private Transform m_transform;
    private List<int> interacting = new();
    private List<int> no_interaction = new();

    public void Start()
    {
        for(int i = 0;i < interactorList.Count;i++) {
            Interactor interactor = interactorList[i];
            locations.Add(interactor.GetComponent<Transform>());
            no_interaction.Add(i);
        }
        m_transform = GetComponent<Transform>();
    }

    public virtual void Interact(Interactor interactor) { }

    public void Update()
    {
        // Hide self from interactors out of range
        for(int i = interacting.Count - 1; i >=0;i--) {
            int j = interacting[i];
            Transform t = locations[j];
            if (Vector2.Distance(t.position, m_transform.position) > interactRadius.Value) {
                no_interaction.Add(j);
                interacting.RemoveAt(i);
                Interactor interactor = interactorList[j];
                interactor.RemoveInteractable(this);
            }
        }

        // Add to interactors in range
        for (int i = no_interaction.Count - 1; i >= 0; i--)
        {
            int j = no_interaction[i];
            Transform t = locations[j];
            if (Vector2.Distance(t.position, m_transform.position) <= interactRadius.Value)
            {
                no_interaction.RemoveAt(i);
                interacting.Add(j);
                Interactor interactor = interactorList[j];
                interactor.AddInteractable(this);
            }
        }
    }

}
