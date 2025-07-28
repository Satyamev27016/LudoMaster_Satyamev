//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class PlayerPiece : MonoBehaviour
//{
//    public bool isReady;
//    public int numberOfStepsAlreadyMove;
//    public int numberOfStepsToMove;
//    public PathsObjectParent pathParent;
//    private Coroutine playerMovement;
//    public PathPoint previousPathPoint;
//    private PathPoint currentPathPoint;
//    public RollingDice myRollingDice;

//    private void Awake()
//    {
//        pathParent = FindFirstObjectByType<PathsObjectParent>();
//    }

//    public void MakePlayerReadyToMove(PathPoint[] pathParent_)
//    {
//        isReady = true;
//        transform.position = pathParent_[0].transform.position;
//        numberOfStepsAlreadyMove = 1;
//        previousPathPoint = pathParent_[0];
//        currentPathPoint = pathParent_[0];

//        // Play single step sound when coming out of base
//        if (AudioManager.instance != null)
//        {
//            AudioManager.instance.PlayStepSound();
//        }

//        bool wasKnockOrCompletion = currentPathPoint.AddPlayerPiece(this);
//        GameManager.gm.AddPathPoint(currentPathPoint);
//        GameManager.gm.HandlePlayerMoveComplete(wasKnockOrCompletion);

//        if (playerMovement != null)
//        {
//            StopCoroutine(playerMovement);
//            playerMovement = null;
//        }
//    }

//    public void MovePlayer(PathPoint[] pathParent_)
//    {
//        if (playerMovement != null)
//        {
//            StopCoroutine(playerMovement);
//        }
//        playerMovement = StartCoroutine(MoveSteps(pathParent_));
//    }

//    IEnumerator MoveSteps(PathPoint[] pathParent_)
//    {
//        yield return new WaitForSeconds(0.25f);
//        numberOfStepsToMove = GameManager.gm.numberOfStepsToMove;

//        for (int i = numberOfStepsAlreadyMove; i < (numberOfStepsAlreadyMove + numberOfStepsToMove); i++)
//        {
//            if (i < pathParent_.Length)
//            {
//                // PLAY STEP SOUND FOR EACH MOVEMENT
//                if (AudioManager.instance != null)
//                {
//                    AudioManager.instance.PlayStepSound();
//                }

//                transform.position = pathParent_[i].transform.position;
//                yield return new WaitForSeconds(0.25f);
//            }
//            else
//            {
//                Debug.LogWarning("PlayerPiece tried to move beyond path length. Stopping movement.");
//                break;
//            }
//        }

//        numberOfStepsAlreadyMove += numberOfStepsToMove;
//        GameManager.gm.RemovePathPoint(previousPathPoint);
//        previousPathPoint.RemovePlayerPiece(this);
//        int finalPathIndex = numberOfStepsAlreadyMove - 1;
//        bool wasKnockOrCompletion = false;

//        if (finalPathIndex >= 0 && finalPathIndex < pathParent_.Length)
//        {
//            currentPathPoint = pathParent_[finalPathIndex];
//            GameManager.gm.AddPathPoint(currentPathPoint);
//            wasKnockOrCompletion = currentPathPoint.AddPlayerPiece(this);
//            currentPathPoint.rescaleAndRepositioningAllPlayerPieces();
//            previousPathPoint = currentPathPoint;
//        }
//        else
//        {
//            wasKnockOrCompletion = false;
//            Debug.Log(gameObject.name + " completed its path!");
//        }

//        GameManager.gm.HandlePlayerMoveComplete(wasKnockOrCompletion);

//        if (playerMovement != null)
//        {
//            StopCoroutine(playerMovement);
//            playerMovement = null;
//        }
//    }

//    bool isPathPointAvailableToMove(int numberOfStepsToMove, int numberOfStepsAlreadyMove, PathPoint[] pathParent_)
//    {
//        return true;
//    }
//}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPiece : MonoBehaviour
{
    [Header("Game Logic")]
    public bool isReady;
    public int numberOfStepsAlreadyMove;
    public int numberOfStepsToMove;
    public PathsObjectParent pathParent;
    public PathPoint previousPathPoint;
    private PathPoint currentPathPoint;
    public RollingDice myRollingDice;
    public RingUVRotate playerRing;

    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float stepDelay = 0.25f;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Ludo King Trail Effect")]
    [SerializeField] private bool createTrailEffect = true;
    [SerializeField] private Material trailMaterial;
    [SerializeField] private float trailDuration = 3f;
    [SerializeField] private float trailSize = 0.8f;
    [SerializeField] private Color trailColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
    [SerializeField] private float trailHeightOffset = 0.1f; // Adjustable height offset

    private List<GameObject> trailObjects = new List<GameObject>();
    private Coroutine playerMovement;
    private bool isMoving = false;

    private void Awake()
    {
        pathParent = FindFirstObjectByType<PathsObjectParent>();

        // Create trail material if not assigned
        if (createTrailEffect && trailMaterial == null)
        {
            CreateTrailMaterial();
        }
    }

    void CreateTrailMaterial()
    {
        // Try Sprites/Default first (works well for 2D-like effects)
        trailMaterial = new Material(Shader.Find("Sprites/Default"));

        if (trailMaterial.shader == null || trailMaterial.shader.name == "Hidden/InternalErrorShader")
        {
            // Fallback to Unlit/Transparent
            trailMaterial = new Material(Shader.Find("Unlit/Transparent"));
        }

        if (trailMaterial.shader == null || trailMaterial.shader.name == "Hidden/InternalErrorShader")
        {
            // Final fallback to Legacy shader
            trailMaterial = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        }

        trailMaterial.color = trailColor;
        trailMaterial.renderQueue = 3000; // Ensure it renders after opaque objects
    }

    public void MakePlayerReadyToMove(PathPoint[] pathParent_)
    {
        isReady = true;

        // Clear any existing trail when starting
        ClearAllTrailEffects();

        // Smooth movement to starting position
        StartCoroutine(SmoothMoveToPosition(pathParent_[0].transform.position, () => {
            numberOfStepsAlreadyMove = 1;
            previousPathPoint = pathParent_[0];
            currentPathPoint = pathParent_[0];

            // Play single step sound when coming out of base
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayStepSound();
            }

            bool wasKnockOrCompletion = currentPathPoint.AddPlayerPiece(this);
            GameManager.gm.AddPathPoint(currentPathPoint);
            GameManager.gm.HandlePlayerMoveComplete(wasKnockOrCompletion);
        }));

        if (playerMovement != null)
        {
            StopCoroutine(playerMovement);
            playerMovement = null;
        }
    }

    public void MovePlayer(PathPoint[] pathParent_)
    {
        if (playerMovement != null)
        {
            StopCoroutine(playerMovement);
        }
        playerMovement = StartCoroutine(MoveSteps(pathParent_));
    }

    IEnumerator MoveSteps(PathPoint[] pathParent_)
    {
        yield return new WaitForSeconds(0.25f);

        isMoving = true;
        numberOfStepsToMove = GameManager.gm.numberOfStepsToMove;

        // Calculate timing for synchronized audio
        float moveDuration = 1f / moveSpeed;
        float totalStepTime = moveDuration + stepDelay;

        // Start synchronized sound sequence
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMovementSounds(numberOfStepsToMove, totalStepTime);
        }

        for (int i = numberOfStepsAlreadyMove; i < (numberOfStepsAlreadyMove + numberOfStepsToMove); i++)
        {
            if (i < pathParent_.Length)
            {
                // Create trail effect at current position before moving
                if (createTrailEffect)
                {
                    CreateTrailAtPosition(transform.position);
                }

                Vector3 targetPosition = pathParent_[i].transform.position;

                // Smooth movement to each step
                yield return StartCoroutine(SmoothMoveToPosition(targetPosition));

                // Small delay between steps (sound will play during this time)
                yield return new WaitForSeconds(stepDelay);
            }
            else
            {
                Debug.LogWarning("PlayerPiece tried to move beyond path length. Stopping movement.");
                break;
            }
        }

        // Update game state (your original logic)
        numberOfStepsAlreadyMove += numberOfStepsToMove;
        GameManager.gm.RemovePathPoint(previousPathPoint);
        previousPathPoint.RemovePlayerPiece(this);

        int finalPathIndex = numberOfStepsAlreadyMove - 1;
        bool wasKnockOrCompletion = false;

        if (finalPathIndex >= 0 && finalPathIndex < pathParent_.Length)
        {
            currentPathPoint = pathParent_[finalPathIndex];
            GameManager.gm.AddPathPoint(currentPathPoint);
            wasKnockOrCompletion = currentPathPoint.AddPlayerPiece(this);
            currentPathPoint.rescaleAndRepositioningAllPlayerPieces();
            previousPathPoint = currentPathPoint;
        }
        else
        {
            wasKnockOrCompletion = false;
            Debug.Log(gameObject.name + " completed its path!");
        }

        isMoving = false;
        GameManager.gm.HandlePlayerMoveComplete(wasKnockOrCompletion);

        if (playerMovement != null)
        {
            StopCoroutine(playerMovement);
            playerMovement = null;
        }
    }

    void CreateTrailAtPosition(Vector3 position)
    {
        if (!createTrailEffect) return;

        // Create material if needed
        if (trailMaterial == null)
        {
            CreateTrailMaterial();
        }

        // Create trail object using a circle mesh instead of quad
        GameObject trailObj = new GameObject("TrailEffect_" + gameObject.name);

        // Add MeshFilter and MeshRenderer
        MeshFilter meshFilter = trailObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = trailObj.AddComponent<MeshRenderer>();

        // Create and assign the circle mesh
        meshFilter.mesh = CreateCircleMesh();

        // Position the trail - adjust height based on your game's needs
        Vector3 trailPosition = new Vector3(position.x, position.y + trailHeightOffset, position.z);
        trailObj.transform.position = trailPosition;

        // For top-down view, rotate to face up
        trailObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Scale to desired size
        trailObj.transform.localScale = Vector3.one * trailSize;

        // Apply trail material
        if (meshRenderer != null)
        {
            meshRenderer.material = new Material(trailMaterial);
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.enabled = true;
        }

        // Add to trail list
        trailObjects.Add(trailObj);

        // Enhanced debug logging
        Debug.Log($"Created trail at position: {trailPosition}, Scale: {trailObj.transform.localScale}, Material: {meshRenderer.material.name}, Shader: {meshRenderer.material.shader.name}");

        // Check if object is actually visible
        if (meshRenderer.isVisible)
        {
            Debug.Log("Trail is visible to camera");
        }
        else
        {
            Debug.LogWarning("Trail is NOT visible to camera - check camera position/culling");
        }

        // Start fade out coroutine
        StartCoroutine(FadeOutTrail(trailObj));
    }

    Mesh CreateCircleMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "CircleTrailMesh";

        // Create a simple circle using triangles
        int segments = 16; // Reduced for better performance
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        Vector2[] uvs = new Vector2[segments + 1];

        // Center vertex
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        // Circle vertices
        for (int i = 0; i < segments; i++)
        {
            float angle = i * 2f * Mathf.PI / segments;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * 0.5f, 0f, Mathf.Sin(angle) * 0.5f);
            uvs[i + 1] = new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f);
        }

        // Create triangles (counter-clockwise for correct normal direction)
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = (i + 1) % segments + 1;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    IEnumerator FadeOutTrail(GameObject trailObj)
    {
        if (trailObj == null) yield break;

        MeshRenderer renderer = trailObj.GetComponent<MeshRenderer>();
        if (renderer == null || renderer.material == null) yield break;

        Material instanceMaterial = renderer.material;
        Color originalColor = instanceMaterial.color;

        float elapsedTime = 0f;

        // Wait a bit before starting fade
        yield return new WaitForSeconds(trailDuration * 0.4f);

        // Fade out over time
        float fadeTime = trailDuration * 0.6f;
        while (elapsedTime < fadeTime && trailObj != null && instanceMaterial != null)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeTime);
            instanceMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Clean up
        if (trailObjects.Contains(trailObj))
        {
            trailObjects.Remove(trailObj);
        }

        if (trailObj != null)
        {
            DestroyImmediate(trailObj);
        }
    }

    void ClearAllTrailEffects()
    {
        // Clear all existing trail objects
        for (int i = trailObjects.Count - 1; i >= 0; i--)
        {
            if (trailObjects[i] != null)
            {
                DestroyImmediate(trailObjects[i]);
            }
        }
        trailObjects.Clear();
    }

    IEnumerator SmoothMoveToPosition(Vector3 targetPosition, System.Action onComplete = null)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / moveSpeed;

        // Face movement direction
        Vector3 direction = (targetPosition - startPosition).normalized;
        if (direction != Vector3.zero && direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float curveValue = movementCurve.Evaluate(progress);

            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }

        // Ensure exact final position
        transform.position = targetPosition;

        // Call completion callback
        onComplete?.Invoke();
    }

    bool isPathPointAvailableToMove(int numberOfStepsToMove, int numberOfStepsAlreadyMove, PathPoint[] pathParent_)
    {
        return true;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    private void OnDestroy()
    {
        // Clean up trail effects when player is destroyed
        ClearAllTrailEffects();
    }
}