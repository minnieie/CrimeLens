using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public Transform doorA;
    public Transform doorB;
    public Vector3 doorARotation = new Vector3(0f, -90f, 0f); // Move A left
    public Vector3 doorBRotation = new Vector3(0f, 90f, 0f);  // Move B right

    public bool requiresKeyPress = false; // Toggle this in Inspector

    private Vector3 doorAClosedRot, doorAOpenRot;
    private Vector3 doorBClosedRot, doorBOpenRot;

    private bool isOpen = false;
    private bool playerInRange = false;

    void Start()
    {
        doorAClosedRot = doorA.rotation.eulerAngles;
        doorBClosedRot = doorB.rotation.eulerAngles;

        doorAOpenRot = doorAClosedRot + doorARotation;
        doorBOpenRot = doorBClosedRot + doorBRotation;
    }

    void Update()
    {
        if (requiresKeyPress && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!requiresKeyPress)
            {
                OpenDoors(); // Auto-open
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (!requiresKeyPress)
            {
                CloseDoors(); // Auto-close
            }
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            OpenDoors();
        }
        else
        {
            CloseDoors();
        }
    }

    public void OpenDoors()
    {
        doorA.rotation = Quaternion.Euler(doorAOpenRot);
        doorB.rotation = Quaternion.Euler(doorBOpenRot);
        isOpen = true;
    }

    public void CloseDoors()
    {
        doorA.rotation = Quaternion.Euler(doorAClosedRot);
        doorB.rotation = Quaternion.Euler(doorBClosedRot);
        isOpen = false;
    }
}
