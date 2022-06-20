using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIDisplayer : MonoBehaviour
{
    public Entity player;
    public Slider healthSlider;
    public int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = player.GetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = player.GetHealth();
    }
}
