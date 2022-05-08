using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Entity player;
    public Slider healthSlider;
    public int maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = Setting.TARGET_FRAME_RATE;
        QualitySettings.vSyncCount = 0;
        ResourceStorage.LoadResource();
        healthSlider.maxValue = maxHealth;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ActionInstance.ExecuteActions();
        healthSlider.value = player.GetHealth();
    }
}
