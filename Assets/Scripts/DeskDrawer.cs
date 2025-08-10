// This script is about controlling a desk drawer in Unity.
// It allows the drawer to open and close smoothly when interacted with.
// The drawer moves in the opposite direction of its forward vector, ensuring consistent behavior regardless of orientation.
// Useful for interactive furniture in first-person or VR environments.

using UnityEngine;

public class DeskDrawer : MonoBehaviour
{
    [Header("Drawer Settings")]
    public Transform drawerPart; // The part of the desk that moves (e.g., the drawer mesh)
    public float openDistance = 0.1f; // How far the drawer should slide out when opened
    public float openSpeed = 3f; // Speed at which the drawer moves

    private Vector3 closedPos; // Local position when the drawer is closed
    private Vector3 openPos;   // Local position when the drawer is fully open
    private bool isOpen = false; // Tracks whether the drawer is currently open
    private bool isMoving = false; // Tracks whether the drawer is in motion

    void Start()
    {
        // Save the initial local position of the drawer (closed state)
        closedPos = drawerPart.localPosition;

        // Get the forward direction of the drawer in world space
        Vector3 worldForward = drawerPart.forward.normalized;

        // Convert world forward to local space and reverse it to get the outward direction
        // This ensures the drawer opens away from the desk, regardless of orientation
        openPos = closedPos - drawerPart.InverseTransformDirection(worldForward) * openDistance;
    }

    void Update()
    {
        // If the drawer is currently moving, interpolate its position
        if (isMoving)
        {
            // Choose the target position based on whether it's opening or closing
            Vector3 targetPos = isOpen ? openPos : closedPos;

            // Move the drawer smoothly toward the target position
            drawerPart.localPosition = Vector3.MoveTowards(
                drawerPart.localPosition,
                targetPos,
                openSpeed * Time.deltaTime
            );

            // Stop moving once the drawer reaches the target position
            if (Vector3.Distance(drawerPart.localPosition, targetPos) < 0.001f)
            {
                drawerPart.localPosition = targetPos;
                isMoving = false;
            }
        }

        // Visual debug: draw a red ray showing the drawer's forward direction and open distance
        Debug.DrawRay(drawerPart.position, drawerPart.forward * openDistance, Color.red);
    }

    // Call this method to toggle the drawer open/closed
    public void Interact()
    {
        // Prevent interaction while the drawer is still moving
        if (isMoving) return;

        // Toggle the open state
        isOpen = !isOpen;

        // Begin moving the drawer
        isMoving = true;

        // Log the drawer's new state to the console
        Debug.Log("Desk drawer " + (isOpen ? "opened" : "closed"));
    }
}
