using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIDisplayer : MonoBehaviour
{
    public Entity player;
    public Slider healthSlider;
    public Slider manaSlider;
    public Actionable actionComponent;

    public Interactor interactor;
    private Mana manaComponent;

    [Header("Interaction UI")]
    [SerializeField] private TMP_Text interactionPrompt1;
    [SerializeField] private TMP_Text interactionPrompt2;
    [SerializeField] private TMP_Text interactionPrompt3;
    [SerializeField] private TMP_Text interactionPrompt4;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.minValue = 0;
        healthSlider.maxValue = player.MaxHealth;
        healthSlider.value = player.Health;

        manaComponent = actionComponent.GetActionComponent<Mana>();
        manaSlider.maxValue = manaComponent.MaxMana;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = player.Health;
        manaSlider.value = manaComponent.AvailableMana;
    }

    private void LateUpdate()
    {
        if (interactionPrompt1 == null)
        {
            return;
        }
        DisplayInteractionPrompt();
    }

    private void DisplayInteractionPrompt()
    {
        InteractionPrompt prompt = interactor.GetInteractionOptions();

        if (prompt.Primary != default)
        {
            interactionPrompt1.text = prompt.Primary;
            interactionPrompt1.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt1.gameObject.SetActive(false);
        }

        if (prompt.Secondary != default)
        {
            interactionPrompt2.text = prompt.Secondary;
            interactionPrompt2.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt2.gameObject.SetActive(false);
        }

        if (prompt.Tertiary != default)
        {
            interactionPrompt3.text = prompt.Tertiary;
            interactionPrompt3.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt3.gameObject.SetActive(false);
        }

        if (prompt.Quaternary != default)
        {
            interactionPrompt4.text = prompt.Quaternary;
            interactionPrompt4.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt4.gameObject.SetActive(false);
        }
    }
}
