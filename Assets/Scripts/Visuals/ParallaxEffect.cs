using UnityEngine;

/// <summary>
/// Moves the GameObject it's attached to in the opposite direction of the mouse cursor
/// to create a simple and effective parallax depth effect.
/// </summary>
public class ParallaxEffect : MonoBehaviour
{
    // How strongly the object moves with the mouse.
    // 0 = no movement. 1 = moves perfectly with the mouse.
    [Range(0f, 1f)]
    public float parallaxStrength;

    private Vector3 startPosition;
    private Camera mainCamera;

    void Start()
    {
        // Store the initial position of this object.
        startPosition = transform.position;
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Ensure we have a camera reference.
        if (mainCamera == null) return;

        // Calculate the mouse position as a value from 0 to 1 for both X and Y.
        Vector3 mousePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);

        // Calculate the desired offset from the center of the screen (which is at 0.5, 0.5).
        float offsetX = (mousePos.x - 0.5f);
        float offsetY = (mousePos.y - 0.5f);

        // Calculate the position offset based on parallax strength.
        // The negative sign makes the background move opposite to the mouse, which feels natural.
        // We multiply by a factor (e.g., 5f) to make the effect more pronounced.
        Vector3 positionOffset = new Vector3(offsetX, offsetY, 0) * -parallaxStrength * 5f;

        // Apply the new position relative to where the object started.
        transform.position = startPosition + positionOffset;
    }
}