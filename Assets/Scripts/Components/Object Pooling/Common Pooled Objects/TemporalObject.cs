using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporalObject : MonoBehaviour
{
    public float duration;
    private float counter;

    public void ResetCounter() {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter >= duration) {
            gameObject.SetActive(false);
            return;
        }
        counter += Time.deltaTime;
    }
}
