using UnityEngine;

// This code is a Unity MonoBehaviour script that controls a two-part door system.
// It allows doors to open and close either automatically when the player enters a trigger zone,
// or manually when the player presses the 'E' key (if enabled).
// It also supports playing a door opening sound when the doors open.

public class DoorBehaviour : MonoBehaviour
{
    public Transform doorA; // Reference to the first door panel
    public Transform doorB; // Reference to the second door panel

    public Vector3 doorARotation = new Vector3(0f, -90f, 0f); // Rotation offset to open doorA (e.g., swing left)
    public Vector3 doorBRotation = new Vector3(0f, 90f, 0f);  // Rotation offset to open doorB (e.g., swing right)

    public bool requiresKeyPress = false; // If true, player must press 'E' to open doors

    // Store closed and open rotation angles
    private Vector3 doorAClosedRot, doorAOpenRot;
    private Vector3 doorBClosedRot, doorBOpenRot;

    private bool isOpen = false; // Tracks whether doors are currently open
    private bool playerInRange = false; // Tracks if player is within trigger zone

    // 🔊 Door sound support
    public AudioSource audioSource; // AudioSource component to play sounds
    public AudioClip doorOpenSound; // Sound to play when doors open

    void Start()
    {
        // Record initial (closed) rotation angles
        doorAClosedRot = doorA.rotation.eulerAngles;
        doorBClosedRot = doorB.rotation.eulerAngles;

        // Calculate open rotation angles by adding offsets
        doorAOpenRot = doorAClosedRot + doorARotation;
        doorBOpenRot = doorBClosedRot + doorBRotation;
    }

    void Update()
    {
        // If key press is required and player is in range, listen for 'E' key
        if (requiresKeyPress && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor(); // Toggle door open/close
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // When player enters trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // If no key press is required, open doors automatically
            if (!requiresKeyPress)
            {
                OpenDoors();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // When player exits trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // If no key press is required, close doors automatically
            if (!requiresKeyPress)
            {
                CloseDoors();
            }
        }
    }

    // Toggle door state between open and closed
    public void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoors();
        }
        else
        {
            OpenDoors();
        }
    }

    // Rotate doors to open position and play sound
    public void OpenDoors()
    {
        doorA.rotation = Quaternion.Euler(doorAOpenRot);
        doorB.rotation = Quaternion.Euler(doorBOpenRot);
        isOpen = true;

        // Play door open sound if assigned
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }
    }

    // Rotate doors back to closed position
    public void CloseDoors()
    {
        doorA.rotation = Quaternion.Euler(doorAClosedRot);
        doorB.rotation = Quaternion.Euler(doorBClosedRot);
        isOpen = false;
    }
}
