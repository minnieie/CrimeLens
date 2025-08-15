using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using System.Collections;

// <summary>
// This script handles player interaction with a computer terminal in a first-person game.
// Interaction prompt and detection are controlled externally via raycast from PlayerBehaviour.
// Pressing 'E' starts the interaction: switches camera, shows UI, disables player movement.
// Ending interaction restores the player state.
// </summary>

public class ComputerBehaviour : MonoBehaviour
{
    [Header("Camera Setup")]
    public CinemachineVirtualCamera sharedZoomCam; // Shared camera that zooms into the computer
    public CinemachineVirtualCamera playerCam;     // Player's default FPS camera
    public Transform zoomTarget;                   // Target position near the computer screen

    [Header("UI and Player")]
    public GameObject computerUI;                  // UI shown during interaction (e.g., quiz)
    public GameObject player;                      // Player GameObject
    public GameObject interactionPrompt;          // UI prompt shown when player is aiming at computer

    [Header("Quiz System")]
    public QuizManager quizManager;                // Reference to quiz logic

    private bool isInteracting = false;            // Tracks if player is currently interacting

    public bool IsInteracting => isInteracting;    // Public read-only access to interaction state
    public bool isUSBOnly = false; // Indicates if this computer is only for USB interaction

    public static ComputerBehaviour ActiveComputer = null; // Tracks currently active computer

    // Removed playerInRange because detection is raycast based now

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

    // Called by PlayerBehaviour when raycast detects the player is aiming at this computer
    public void ShowInteractionPrompt()
    {
        if (!isInteracting && interactionPrompt != null && !interactionPrompt.activeSelf)
            interactionPrompt.SetActive(true);
    }

    // Called by PlayerBehaviour when raycast no longer detects this computer
    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null && interactionPrompt.activeSelf)
            interactionPrompt.SetActive(false);
    }

    public void StartInteraction()
    {
        if (isInteracting) return;

        if (GameManager.instance != null && GameManager.instance.questTrackerUI != null)
        {
            GameManager.instance.questTrackerUI.SetActive(false);
        }

        // Hide interaction prompt just in case
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        ActiveComputer = this;
        isInteracting = true;

        // Hide player renderers for immersion
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

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
        Debug.Log("EndInteraction called on " + gameObject.name);
        isInteracting = false;

        if (GameManager.instance != null && GameManager.instance.questTrackerUI != null)
        {
            GameManager.instance.questTrackerUI.SetActive(true);
        }

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

    private IEnumerator ReenablePlayerRenderersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
    }
}
