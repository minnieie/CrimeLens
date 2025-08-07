using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using System.Collections;


public class ComputerBehaviour : MonoBehaviour
{
    [Header("Camera Setup")]
    public CinemachineVirtualCamera sharedZoomCam; // Assign one shared cam in the scene
    public CinemachineVirtualCamera playerCam;     // Your FPS player camera
    public Transform zoomTarget;                   // This is the target near the computer screen

    [Header("UI and Player")]
    public GameObject computerUI;                  // UI to show during interaction
    public GameObject player;                      // Your player GameObject

    public MonoBehaviour lookScript; // Assign your look script in the Inspector

    private bool isInteracting = false;

    public bool IsInteracting => isInteracting;

    public static ComputerBehaviour ActiveComputer = null;

    [Header("Quiz System")]
    public QuizManager quizManager;

    void Start()
    {
        // Set correct camera priorities
        if (playerCam != null) playerCam.Priority = 20;
        if (sharedZoomCam != null) sharedZoomCam.Priority = 10;

        // Hide computer UI at start
        if (computerUI != null) computerUI.SetActive(false);

        // Enable player movement at start
        if (player != null)
            player.GetComponent<FirstPersonController>().enabled = true;

        isInteracting = false;
    }

    public void StartInteraction()
    {
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        if (isInteracting) return;
        ActiveComputer = this;

        Debug.Log("StartInteraction called on " + gameObject.name);
        isInteracting = true;

        // Point sharedZoomCam to this computer's zoom target
        if (sharedZoomCam != null && zoomTarget != null)
        {
            sharedZoomCam.LookAt = zoomTarget;
            sharedZoomCam.Follow = zoomTarget;
            sharedZoomCam.Priority = 20;
            playerCam.Priority = 10;
            Debug.Log("Switched to sharedZoomCam at " + zoomTarget.name);
        }
        else
        {
            Debug.LogWarning("sharedZoomCam or zoomTarget not assigned!");
        }

        // Show computer UI
        if (computerUI != null)
        {
            computerUI.SetActive(true);
        }

        // Start quiz
        if (quizManager != null)
        {
            quizManager.InitializeQuiz(); // resets and shows first question for this computer
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player GameObject
        if (player != null)
            player.GetComponent<FirstPersonController>().enabled = false;
    }

    public void EndInteraction()
    {
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
        {
            computerUI.SetActive(false);
        }

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable player GameObject
        if (player != null)
            player.GetComponent<FirstPersonController>().enabled = true;

        ActiveComputer = null;

        StartCoroutine(ReenablePlayerRenderersAfterDelay(1.5f)); // Delay in seconds

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player in range");
            // Optional: Show interaction prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInteracting)
        {
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