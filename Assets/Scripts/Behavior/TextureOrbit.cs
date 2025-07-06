using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    [Header("Animation Parameters")]
    [Tooltip("How fast the mask orbits its parent.")]
    public float orbitSpeed = 1f;

    [Tooltip("How wide the circular path of the movement is.")]
    public float orbitRadius = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // Calculate the X and Y offset for a circular path based on time.
        float angle = Time.time * orbitSpeed;
        float offsetX = Mathf.Cos(angle) * orbitRadius;
        float offsetY = Mathf.Sin(angle) * orbitRadius;

        // Apply the calculated offset to the object's local position.
        transform.localPosition = startPosition + new Vector3(offsetX, offsetY, 0);
    }
}