using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProceduralShockwave : MonoBehaviour
{
    private float duration;
    private float endScale;
    private Color startColor;
    private float rotationSpeed; // New variable

    private float elapsedTime = 0f;
    private SpriteRenderer spriteRenderer;

    // Updated Initialize method
    public void Initialize(float newDuration, float newEndScale, Color newStartColor, float newRotationSpeed)
    {
        this.duration = newDuration;
        this.endScale = newEndScale;
        this.startColor = newStartColor;
        this.rotationSpeed = newRotationSpeed; // Set the new variable

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = startColor;
    }

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Animate scale
            transform.localScale = Vector3.one * Mathf.Lerp(0f, endScale, progress);

            // Animate color alpha
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(startColor.a, 0f, progress);
            spriteRenderer.color = newColor;

            // Apply rotation
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}