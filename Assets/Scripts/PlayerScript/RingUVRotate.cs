using UnityEngine;

public class RingUVRotate : MonoBehaviour
{
    [Header("Ring Settings")]
    public Transform player;         // Player token to follow
    public float rotateSpeed = 50f;  // UV rotation speed in degrees per second
    public bool isActive = false;    // Should rotate?

    [Header("Optional")]
    public Vector3 offset = Vector3.zero; // Offset from player position

    // Internal variables
    private Vector3 lastPlayerPosition;
    private Mesh meshInstance;
    private MeshFilter meshFilter;
    private Vector2[] originalUVs;
    private float currentRotation = 0f;

    void Start()
    {
        // Get the mesh filter component
        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("RingUVRotate: No MeshFilter component found on " + gameObject.name);
            return;
        }

        // Create mesh instance to avoid affecting other objects
        meshInstance = Instantiate(meshFilter.sharedMesh);
        meshFilter.mesh = meshInstance;

        // Store original UV coordinates
        originalUVs = meshInstance.uv;

        // Initialize position if player is set
        if (player != null)
        {
            transform.position = player.position + offset;
            lastPlayerPosition = player.position;
        }
    }

    void Update()
    {
        // Follow player position
        FollowPlayer();

        // Rotate UV coordinates
        RotateUV();
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            // Only update position if player actually moved
            if (Vector3.Distance(player.position, lastPlayerPosition) > 0.001f)
            {
                transform.position = player.position + offset;
                lastPlayerPosition = player.position;
            }
        }
    }

    private void RotateUV()
    {
        if (isActive && meshInstance != null && originalUVs != null)
        {
            // Update rotation angle
            currentRotation += rotateSpeed * Time.deltaTime;

            // Keep rotation in 0-360 range
            if (currentRotation >= 360f)
                currentRotation -= 360f;

            // Convert to radians
            float rotationRad = currentRotation * Mathf.Deg2Rad;

            // Create new UV array
            Vector2[] newUVs = new Vector2[originalUVs.Length];

            // Rotate each UV coordinate around center (0.5, 0.5)
            for (int i = 0; i < originalUVs.Length; i++)
            {
                Vector2 uv = originalUVs[i];

                // Translate to center
                uv.x -= 0.5f;
                uv.y -= 0.5f;

                // Apply rotation
                float cos = Mathf.Cos(rotationRad);
                float sin = Mathf.Sin(rotationRad);

                float newX = uv.x * cos - uv.y * sin;
                float newY = uv.x * sin + uv.y * cos;

                // Translate back
                newX += 0.5f;
                newY += 0.5f;

                newUVs[i] = new Vector2(newX, newY);
            }

            // Apply new UVs to mesh
            meshInstance.uv = newUVs;
        }
    }

    public void ActivateRing(bool status)
    {
        isActive = status;
        gameObject.SetActive(status);

        // Reset UV rotation when activating
        if (status && meshInstance != null && originalUVs != null)
        {
            currentRotation = 0f;
            meshInstance.uv = originalUVs;
        }
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        if (player != null)
        {
            transform.position = player.position + offset;
            lastPlayerPosition = player.position;
        }
    }

    public void SetRotationSpeed(float newSpeed)
    {
        rotateSpeed = newSpeed;
    }

    public void ResetUVRotation()
    {
        currentRotation = 0f;
        if (meshInstance != null && originalUVs != null)
        {
            meshInstance.uv = originalUVs;
        }
    }

    // Clean up mesh instance
    void OnDestroy()
    {
        if (meshInstance != null)
        {
            DestroyImmediate(meshInstance);
        }
    }

    // Optional: Smooth speed transitions
    public void ChangeRotationSpeed(float targetSpeed, float duration)
    {
        StartCoroutine(SmoothSpeedChange(targetSpeed, duration));
    }

    private System.Collections.IEnumerator SmoothSpeedChange(float targetSpeed, float duration)
    {
        float startSpeed = rotateSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            rotateSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            yield return null;
        }

        rotateSpeed = targetSpeed;
    }
}