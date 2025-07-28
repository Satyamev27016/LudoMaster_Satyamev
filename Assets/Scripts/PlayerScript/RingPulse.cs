using UnityEngine;

public class RingPulse : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; // Speed of rotation

    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;     // How fast the pulse happens
    public float minAlpha = 0.4f;     // Minimum transparency
    public float maxAlpha = 1f;       // Maximum transparency

    private Material ringMaterial;
    private Color originalColor;

    void Start()
    {
        // Get the material of the ring
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Make a copy of the material to avoid changing all instances
            ringMaterial = renderer.material;
            originalColor = ringMaterial.color;
        }
    }

    void Update()
    {
        // Rotate the ring
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Create pulse (fade in and out)
        if (ringMaterial != null)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            Color newColor = originalColor;
            newColor.a = alpha;
            ringMaterial.color = newColor;
        }
    }
}
