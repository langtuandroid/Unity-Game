using UnityEngine;
using UnityEngine.UI;
using LobsterFramework;
using System.Collections.Generic;

namespace LobsterFramework.AI
{
    public class EntityAlertBar : MonoBehaviour
    {
        private CanvasGroup canvas;
        [SerializeField] private Slider slider = null;

        // Update is called once per frame
        private void Start()
        {
            canvas = GetComponent<CanvasGroup>();
            canvas.alpha = 0;
        }
        void Update()
        {

        }
        public void SetAlert(float value)
        {
            slider.value += value;
            canvas.alpha = 1;
            Debug.Log("increase");
        }
        public void Hide()
        {
            Debug.Log("decrease");
            canvas.alpha = 0;
            slider.value = 0;
        }
    }
}


