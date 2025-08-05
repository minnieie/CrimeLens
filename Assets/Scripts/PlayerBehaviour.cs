using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    bool canInteract; // Flag to check if player can interact with objects
    CoinBehaviour nearbyCoin; // The coin the player is near
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
        //currentHidingSpot = null;

        RaycastHit hitInfo;
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.magenta);
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.CompareTag("Collectible"))
            {
                CoinBehaviour coin = hitInfo.collider.gameObject.GetComponent<CoinBehaviour>();
                if (coin != null)
                {
                    canInteract = true;
                    nearbyCoin = coin; // Set the nearby coin
                    if (coin != lastCoin)
                    {
                        lastCoin?.Unhighlight();
                    }
                    nearbyCoin.Highlight(); // Highlight the coin
                    lastCoin = coin; // Update the last coin to the current one
                }
            }

            // else if (hitInfo.collider.gameObject.CompareTag("HidingSpot"))
            // {
            //     canInteract = true;
            //     currentHidingSpot = hitInfo.collider.gameObject.GetComponent<HidingSpot>();
            // }
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
        }
    }
}
