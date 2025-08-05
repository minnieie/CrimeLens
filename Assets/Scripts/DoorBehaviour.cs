using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public Transform doorA;
    public Transform doorB;
    public Vector3 doorARotation = new Vector3(0f, -90f, 0f); // Move A left
    public Vector3 doorBRotation = new Vector3(0f, 90f, 0f);  // Move B right

    private Vector3 doorAClosedRot, doorAOpenRot;
    private Vector3 doorBClosedRot, doorBOpenRot;

    private bool isOpen = false;

    void Start()
    {
        doorAClosedRot = doorA.rotation.eulerAngles;
        doorBClosedRot = doorB.rotation.eulerAngles;

        doorAOpenRot = doorAClosedRot + doorARotation;
        doorBOpenRot = doorBClosedRot + doorBRotation;
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
