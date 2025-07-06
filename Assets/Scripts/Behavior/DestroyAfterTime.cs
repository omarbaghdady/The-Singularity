using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [Tooltip("The lifetime of this object in seconds.")]
    public float lifetime = 1f;

    void Start()
    {
        // Destroy this GameObject after 'lifetime' seconds.
        Destroy(gameObject, lifetime);
    }
}