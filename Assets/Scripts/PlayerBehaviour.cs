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
        canInteract = false; // Reset interaction flag each frame
        //currentHidingSpot = null;

        RaycastHit hitInfo;
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.magenta);
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.CompareTag("Collectible"))
            {
                // Set the canInteract flag to true
                // Get the Collectible component from the detected object
                canInteract = true;
                nearbyCoin = hitInfo.collider.gameObject.GetComponent<CoinBehaviour>();
                nearbyCoin.Highlight(); // Highlight the coin
            }

            // else if (hitInfo.collider.gameObject.CompareTag("HidingSpot"))
            // {
            //     canInteract = true;
            //     currentHidingSpot = hitInfo.collider.gameObject.GetComponent<HidingSpot>();
            // }
            else
            {
                nearbyCoin.Unhighlight(); // Unhighlight if not a collectible
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
