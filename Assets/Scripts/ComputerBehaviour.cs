using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using System.Collections;

// <summary>
// This script handles player interaction with a computer terminal in a first-person game.
// When the player enters the trigger zone, a UI prompt appears.
// Pressing 'E' starts the interaction: switches camera, shows UI, disables player movement.
// Exiting the trigger zone or ending interaction restores the player state.

public class ComputerBehaviour : MonoBehaviour
{
    [Header("Camera Setup")]
    public CinemachineVirtualCamera sharedZoomCam; // Shared camera that zooms into the computer
    public CinemachineVirtualCamera playerCam;     // Player's default FPS camera
    public Transform zoomTarget;                   // Target position near the computer screen

    [Header("UI and Player")]
    public GameObject computerUI;                  // UI shown during interaction (e.g., quiz)
    public GameObject player;                      // Player GameObject
    public GameObject interactionPrompt;           // UI prompt shown when player is in range

    [Header("Quiz System")]
    public QuizManager quizManager;                // Reference to quiz logic

    private bool isInteracting = false;            // Tracks if player is currently interacting
    private bool playerInRange = false;            // Tracks if player is in trigger zone

    public bool IsInteracting => isInteracting;    // Public read-only access to interaction state
    public static ComputerBehaviour ActiveComputer = null; // Tracks currently active computer

    void Start()
    {
        // Set initial camera priorities
        if (playerCam != null) playerCam.Priority = 20;
        if (sharedZoomCam != null) sharedZoomCam.Priority = 10;

        // Hide computer UI and interaction prompt at start
        if (computerUI != null) computerUI.SetActive(false);
        if (interactionPrompt != null) interactionPrompt.SetActive(false);

        // Enable player movement
        if (player != null)
            player.GetComponent<FirstPersonController>().enabled = true;

        isInteracting = false;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            Debug.Log($"Distance to computer: {distance}");
            if (distance <= 3f)  // or your interactionDistance
            {
                StartInteraction();
            }
            else
            {
                Debug.Log("Too far from computer to interact");
            }
        }
    }


    public void StartInteraction()
    {   
         // Hide interaction prompt just in case
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        playerInRange = false; // Prevent prompt from reappearing

        // Hide player renderers for immersion
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        if (isInteracting) return;
        ActiveComputer = this;
        isInteracting = true;

        Debug.Log("StartInteraction called on " + gameObject.name);

        // Switch to zoom camera
        if (sharedZoomCam != null && zoomTarget != null)
        {
            sharedZoomCam.LookAt = zoomTarget;
            sharedZoomCam.Follow = zoomTarget;
            sharedZoomCam.Priority = 20;
            playerCam.Priority = 10;
        }
        else
        {
            Debug.LogWarning("sharedZoomCam or zoomTarget not assigned!");
        }

        // Show computer UI
        if (computerUI != null)
            computerUI.SetActive(true);

        // Initialize quiz
        if (quizManager != null)
            quizManager.InitializeQuiz();

        // Show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement
        if (player != null)
            player.GetComponent<FirstPersonController>().enabled = false;
    }

    public void EndInteraction()
    {
        // Re-enable player renderer
        MeshRenderer playerRenderer = player.GetComponent<MeshRenderer>();
        if (playerRenderer != null)
            playerRenderer.enabled = true;

        Debug.Log("EndInteraction called on " + gameObject.name);
        isInteracting = false;

        // Switch back to player camera
        if (sharedZoomCam != null && playerCam != null)
        {
            sharedZoomCam.Priority = 10;
            playerCam.Priority = 20;
        }

        // Hide computer UI
        if (computerUI != null)
            computerUI.SetActive(false);

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Re-enable player movement
        if (player != null)
            player.GetComponent<FirstPersonController>().enabled = true;

        ActiveComputer = null;

        // Delay before showing player renderers again
        StartCoroutine(ReenablePlayerRenderersAfterDelay(1.5f));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Only show prompt if not interacting
            if (!isInteracting && interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Hide interaction prompt
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            // End interaction if player walks away
            if (isInteracting)
                EndInteraction();
        }
    }

    private IEnumerator ReenablePlayerRenderersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
    }
}
