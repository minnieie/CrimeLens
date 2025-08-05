using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    bool canInteract; // Flag to check if player can interact with objects
    CoinBehaviour nearbyCoin; // The coin the player is near
    DoorBehaviour currentDoor; // The door the player is near
    private CoinBehaviour lastCoin = null; // The last coin the player interacted with
    public AudioSource footstepAudio;
    public float moveThreshold = 0.1f;
    private Vector3 lastPosition;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float interactionDistance = 3.0f;

    void Start()
    {
        lastPosition = transform.position;
    }
    void Update()
    {
        canInteract = false; // Reset interaction flag each frame
        // Check if the main camera exists in the scene
        if (Camera.main == null)
        {
            Debug.LogWarning("Main Camera not found!");
            return; // Exit early if no camera found
        }

        // Set the origin of the raycast to the camera's position
        Vector3 rayOrigin = Camera.main.transform.position;
        // Set the direction of the raycast to where the camera is facing
        Vector3 rayDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.magenta);
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, interactionDistance))
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);

            // Check if the object hit by the raycast has the "Collectible" tag
            if (hitInfo.collider.gameObject.CompareTag("Collectible"))
            {
                // Set the canInteract flag to true
                // Get the Collectible component from the detected object
                canInteract = true;
                nearbyCoin = hitInfo.collider.gameObject.GetComponent<CoinBehaviour>();
                nearbyCoin.Highlight(); // Highlight the coin
            }

            else if (hitInfo.collider.gameObject.CompareTag("Door"))
            {
                canInteract = true;
                currentDoor = hitInfo.collider.gameObject.GetComponent<DoorBehaviour>();
            }
            else
            {
                if (lastCoin != null)
                {
                    lastCoin.Unhighlight(); // Unhighlight the last coin if not a collectible
                    lastCoin = null;
                }
                nearbyCoin = null;
                currentDoor = null; // Reset current door if not a door
            }
        }
        else
        {
            if (lastCoin != null)
                {
                    lastCoin.Unhighlight(); // Unhighlight the last coin if not a collectible
                    lastCoin = null;
                }
                nearbyCoin = null;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        // Level Change (still happens automatically on trigger)
        LevelChanger levelChanger = other.GetComponent<LevelChanger>();
        if (levelChanger != null)
        {
            // LevelChanger handles scene loading
            return;
        }
    }

    // void OnTriggerStay(Collider other)
    // {
    //     // When staying near a coin
    //     CoinBehaviour coin = other.GetComponent<CoinBehaviour>();
    //     if (coin != null)
    //     {
    //         nearbyCoin = coin;
    //         coin.Highlight();
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     // When walking away from the coin
    //     CoinBehaviour coin = other.GetComponent<CoinBehaviour>();
    //     if (coin != null && coin == nearbyCoin)
    //     {
    //         coin.Unhighlight();
    //         nearbyCoin = null;
    //     }
    // }

    public void OnInteract()
    {
        if (canInteract)
        {
            if (nearbyCoin != null)
            {
                Debug.Log("Interacting with coin: " + nearbyCoin.gameObject.name);
                nearbyCoin.Collect(this); // Collect the coin
            }
            else if (currentDoor != null)
            {
                Debug.Log("Interacting with door: " + currentDoor.gameObject.name);
                currentDoor.OpenDoors();
            }
        }
    }
}
