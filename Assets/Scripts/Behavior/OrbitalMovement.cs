using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsOrbitalMovement : MonoBehaviour
{
    public static int ActiveObjectCount { get; private set; }

    [Header("Orbit Parameters")]
    public Transform orbitCenter;
    public float orbitSpeed = 5f;
    public float gravityStrength = 5f;

    [Header("Attraction")]
    public float pullMultiplier = 10f;
    public float pullDuration = 0.2f;

    [Header("Absorption")]
    public GameObject absorptionVFX; 

    private Rigidbody2D rb;
    private SingularityController singularity;
    private float pullTimer;

    private void OnEnable()
    {
        SingularityController.OnSingularityClicked += HandleAttraction;
        ActiveObjectCount++;
    }

    private void OnDisable()
    {
        SingularityController.OnSingularityClicked -= HandleAttraction;
        ActiveObjectCount--;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        singularity = FindObjectsByType<SingularityController>(FindObjectsSortMode.None)[0];
        orbitCenter = singularity.transform;
        Vector2 directionToCenter = (orbitCenter.position - transform.position).normalized;
        Vector2 initialVelocity = new Vector2(-directionToCenter.y, directionToCenter.x) * orbitSpeed;
        rb.linearVelocity = initialVelocity;
    }

    void FixedUpdate()
    {
        Vector2 direction = (orbitCenter.position - transform.position).normalized;

        float currentGravity = gravityStrength;
        if (pullTimer > 0)
        {
            currentGravity *= pullMultiplier;
            pullTimer -= Time.fixedDeltaTime;
        }

        rb.AddForce(direction * currentGravity);
        rb.linearVelocity *= 0.999f;
    }

    void HandleAttraction()
    {
        pullTimer = pullDuration;
    }

void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Singularity"))
    {
        if (singularity != null)
        {
            singularity.NotifyOfAbsorption();
                singularity.PlayAbsorptionSound();
            }

        // If an effect prefab is assigned, create it and set its color
        if (absorptionVFX != null)
        {
            // Get the color from this object's own sprite renderer before destroying it
            Color objectColor = GetComponent<SpriteRenderer>().color;

            // Instantiate the particle effect
            GameObject effectInstance = Instantiate(absorptionVFX, transform.position, Quaternion.identity);

            // Get the particle system from the instance and set its start color
            var main = effectInstance.GetComponent<ParticleSystem>().main;
            main.startColor = objectColor;
        }

        Destroy(gameObject);
    }
}
}