using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Coroutines/ColorChange")]
public class ColorChangeCoroutine : CoroutineOperationSO
{
    [SerializeField] private Color targetColor;
    [Header("Exposed Reference<Sprite Renderer>")]
    [SerializeField] private string rendererID;
    [SerializeField] private float speed;

    private SpriteRenderer spriteRenderer;

    protected override IEnumerator Execute()
    {
        if (spriteRenderer == null) {
            ExposedReference<SpriteRenderer> reference = new();
            reference.exposedName = rendererID;
            spriteRenderer = reference.Resolve(ExposedPropertyTable.Instance);
        }
        Color color = spriteRenderer.color;
        float deltaG = targetColor.g - color.g;
        float deltaB = targetColor.b - color.b;
        float deltaR = targetColor.r - color.r;
        float time = 100 / speed;
        float counter = 0f;
        while (counter < time) {
            counter += Time.deltaTime;
            float percentage = counter / time;
            float r = color.r + deltaR * percentage;
            float g = color.g + deltaG * percentage;
            float b = color.b + deltaB * percentage;
            spriteRenderer.color = new Color(r, g, b);
            yield return null;
        }
        spriteRenderer.color = new(targetColor.r, targetColor.g, targetColor.b);
    }
}
