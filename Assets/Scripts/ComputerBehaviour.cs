using UnityEngine;

public class ComputerInteractor : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;      // Main camera
    public Transform zoomPosition;     // Empty object where camera should zoom to
    public GameObject computerUI;     // UI canvas
    public float zoomSpeed = 5f;      // How fast camera zooms in/out
    public float zoomFOV = 30f;       // Field of view when zoomed in

    private bool isInteracting = false;
    private bool isZooming = false;
    private bool playerInRange = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float originalFOV;
    private Camera cam;

    void Start()
    {
        cam = playerCamera.GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("No Camera component found on playerCamera!");
        }
    }

    void Update()
    {
        // Start interaction with E when in range
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isInteracting)
        {
            StartInteraction();
        }

        // End interaction with either Escape or Q
        if (isInteracting && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            EndInteraction();
        }

        // Handle camera zoom
        if (isZooming)
        {
            ZoomCamera();
        }
    }

    void StartInteraction()
    {
        if (zoomPosition == null || computerUI == null || cam == null)
        {
            Debug.LogError("Required references not set!");
            return;
        }

        isInteracting = true;
        isZooming = true;
        
        // Store original camera values
        originalPosition = playerCamera.position;
        originalRotation = playerCamera.rotation;
        originalFOV = cam.fieldOfView;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement (you'll need to modify this for your player controller)
        // Example: FindObjectOfType<PlayerMovement>().enabled = false;
    }

    void ZoomCamera()
    {
        if (isInteracting) // Zooming in
        {
            // Move camera towards zoom position
            playerCamera.position = Vector3.Lerp(
                playerCamera.position,
                zoomPosition.position,
                Time.deltaTime * zoomSpeed
            );

            // Rotate camera to match zoom position
            playerCamera.rotation = Quaternion.Slerp(
                playerCamera.rotation,
                zoomPosition.rotation,
                Time.deltaTime * zoomSpeed
            );

            // Adjust FOV for zoom effect
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);

            // Check if we're close enough to consider the zoom complete
            if (Vector3.Distance(playerCamera.position, zoomPosition.position) < 0.05f)
            {
                isZooming = false;
                computerUI.SetActive(true);
            }
        }
        else // Zooming out
        {
            // Move camera back to original position
            playerCamera.position = Vector3.Lerp(
                playerCamera.position,
                originalPosition,
                Time.deltaTime * zoomSpeed
            );

            // Rotate camera back to original rotation
            playerCamera.rotation = Quaternion.Slerp(
                playerCamera.rotation,
                originalRotation,
                Time.deltaTime * zoomSpeed
            );

            // Reset FOV
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);

            // Check if we're back to the original position
            if (Vector3.Distance(playerCamera.position, originalPosition) < 0.05f)
            {
                isZooming = false;
                playerCamera.position = originalPosition; // Snap to exact position
                playerCamera.rotation = originalRotation; // Snap to exact rotation
                cam.fieldOfView = originalFOV; // Reset exact FOV
            }
        }
    }

    void EndInteraction()
    {
        if (!isInteracting) return;
        
        isInteracting = false;
        isZooming = true;
        computerUI.SetActive(false);

        // Re-lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Re-enable player movement
        // Example: FindObjectOfType<PlayerMovement>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isInteracting) EndInteraction();
        }
    }
}