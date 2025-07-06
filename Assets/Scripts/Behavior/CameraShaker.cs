using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    // A singleton instance to easily access this script from anywhere.
    public static CameraShaker Instance;

    private Vector3 initialPosition;
    private float shakeMagnitude = 0f;

    void Awake()
    {
        // Set up the singleton instance.
        if (Instance == null)
        {
            Instance = this;
            initialPosition = transform.localPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // If there's shake magnitude, apply it as a random offset.
        if (shakeMagnitude > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            // Dampen the shake over time.
            shakeMagnitude -= Time.deltaTime * 2f; // The '2f' controls how fast the shake fades.
        }
        else
        {
            // Reset to the initial position when the shake is over.
            shakeMagnitude = 0f;
            transform.localPosition = initialPosition;
        }
    }

    // Public method to be called from other scripts to start a shake.
    public void Shake(float magnitude)
    {
        // Set the shake magnitude, ensuring new shakes can be stronger than current ones.
        if (magnitude > shakeMagnitude)
        {
            shakeMagnitude = magnitude;
        }
    }
}