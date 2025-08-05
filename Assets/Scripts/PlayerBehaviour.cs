using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    bool canInteract; // Flag to check if player can interact with objects
    CoinBehaviour nearbyCoin; // The coin the player is near
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
        // Reset the interaction flag at the start of each frame
        canInteract = false;

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

        // Draw a magenta debug ray in the Scene view to visualize the raycast
        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.magenta);

        // Perform the raycast from the camera forward, limited by interactionDistance
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, interactionDistance))
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);

            // Check if the object hit by the raycast has the "Collectible" tag
            if (hitInfo.collider.gameObject.CompareTag("Collectible"))
            {
                canInteract = true; // Player can interact with this object
                // Get the CoinBehaviour script attached to the collectible object
                nearbyCoin = hitInfo.collider.gameObject.GetComponent<CoinBehaviour>();
                nearbyCoin.Highlight(); // Highlight the collectible to show it can be interacted with
            }
            else
            {
                // If the hit object is NOT collectible but a coin was previously highlighted
                if (nearbyCoin != null)
                {
                    nearbyCoin.Unhighlight(); // Remove highlight from the previous collectible
                    nearbyCoin = null;         // Clear the reference so interaction won't happen
                }
            }
        }
        else
        {
            // If raycast didn't hit anything and a collectible was previously highlighted
            if (nearbyCoin != null)
            {
                nearbyCoin.Unhighlight(); // Remove highlight
                nearbyCoin = null;         // Clear the reference
            }
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

    void OnTriggerExit(Collider other)
    {
        // When walking away from the coin
        CoinBehaviour coin = other.GetComponent<CoinBehaviour>();
        if (coin != null && coin == nearbyCoin)
        {
            coin.Unhighlight();
            nearbyCoin = null;
        }
    }

    public void OnInteract()
    {
        if (canInteract)
        {
            if (nearbyCoin != null)
            {
                Debug.Log("Interacting with coin: " + nearbyCoin.gameObject.name);
                nearbyCoin.Collect(this); // Collect the coin
            }
        }
    }
}
