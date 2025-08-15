using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles the behavior of a USB object in a Unity game.
/// It manages picking up the USB, inserting it into a specific computer,
/// and simulating a download process with a progress bar.
/// </summary>
public class USBBehaviour : MonoBehaviour
{
    [Header("USB Settings")]
    public float downloadTime = 30f; // Time in seconds to complete the download from 0% to 100%
    public Slider progressBar;      // Optional UI slider to show download progress

    [Header("Insertion Settings")]
    public Transform allowedComputerTransform;  // Reference to the specific computer the USB can be inserted into
    public float insertDistance = 10f;          // Max distance from computer to allow insertion

    [Header("Pickup Requirement")]
    public DeskDrawer requiredDrawer;           // Reference to a drawer that must be open before USB can be picked up

    [HideInInspector] public bool isPickedUp = false;   // Tracks if the USB has been picked up
    [HideInInspector] public bool isInserted = false;   // Tracks if the USB has been inserted into the computer

    private float currentProgress = 0f;         // Internal tracker for download progress
    private bool isDownloading = false;         // Flag to control download state

    // Reference to player's camera transform for proximity checks
    [HideInInspector] public Transform playerCameraTransform;

    void Update()
    {
        // If USB is downloading, update progress over time
        if (isDownloading)
        {
            currentProgress += Time.deltaTime / downloadTime; // Increment progress based on time
            Debug.Log($"Downloading progress: {currentProgress}");

            // Update UI slider if assigned
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(currentProgress);
            }

            // Check if download is complete
            if (currentProgress >= 1f)
            {
                currentProgress = 1f;
                isDownloading = false;
                OnDownloadComplete(); // Trigger completion logic
            }
        }
    }

    // Called externally (e.g. by PlayerBehaviour) when player tries to pick up the USB.
    // Checks if drawer is open before allowing pickup.
    public void PickUpUSB()
    {
        Debug.Log("PickUpUSB() called");

        // Log drawer state for debugging
        if (requiredDrawer != null)
        {
            Debug.Log("Drawer assigned: " + requiredDrawer.name);
            Debug.Log("Drawer open state: " + requiredDrawer.IsOpen);
        }
        else
        {
            Debug.Log("No drawer assigned to USB.");
        }

        // Prevent pickup if drawer is closed
        if (requiredDrawer != null && !requiredDrawer.IsOpen)
        {
            Debug.Log("Can't pick up USB until drawer is opened!");
            return;
        }

        // Proceed with pickup
        isPickedUp = true;

        // Hide USB visuals and disable colliders
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
            r.enabled = false;

        var colliders = GetComponentsInChildren<Collider>();
        foreach (var c in colliders)
            c.enabled = false;

        Debug.Log("USB picked up!");
    }

    // Called externally when player attempts to insert USB into computer.
    // Checks distance and state before allowing insertion.
    public bool TryInsertIntoComputer()
    {
        // Must be picked up first
        if (!isPickedUp)
        {
            Debug.Log("USB not picked up yet.");
            return false;
        }

        // Prevent double insertion
        if (isInserted)
        {
            Debug.Log("USB already inserted.");
            return false;
        }

        // Ensure required references are set
        if (playerCameraTransform == null || allowedComputerTransform == null)
        {
            Debug.LogWarning("Player camera or allowed computer transform not set.");
            return false;
        }

        // Check distance between player and computer
        float dist = Vector3.Distance(playerCameraTransform.position, allowedComputerTransform.position);
        Debug.Log($"Distance to computer USB port: {dist}");

        if (dist > insertDistance)
        {
            Debug.Log("Too far from computer to insert USB.");
            return false;
        }

        // All checks passed, insert USB
        InsertIntoComputer();
        return true;
    }

    // Handles the actual insertion logic and starts the download.
    private void InsertIntoComputer()
    {
        Debug.Log("InsertIntoComputer() called.");
        isInserted = true;
        isDownloading = true;
        currentProgress = 0f;

        // Reset UI progress
        if (progressBar != null)
            progressBar.value = 0f;

        Debug.Log("USB inserted into computer. Starting download...");
    }

    // Called when download reaches 100%.
    private void OnDownloadComplete()
    {
        Debug.Log("Download complete!");

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int stage = QuestTracker.Instance.GetQuestStage(sceneName);
        int objectiveIndex = 0; // Set the appropriate objective index

        QuestTracker.Instance.CompleteObjective(sceneName, stage, objectiveIndex);
    }
}
