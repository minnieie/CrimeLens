using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private CoinBehaviour nearbyCoin; // The coin the player is near
    public AudioSource footstepAudio;  
    public float moveThreshold = 0.1f; 
    private Vector3 lastPosition;
    

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Check for "E" key press and collect the coin if nearby
        if (nearbyCoin != null && Input.GetKeyDown(KeyCode.E))
        {
            nearbyCoin.Collect(this); // Collect coin
            nearbyCoin = null; // Clear reference after collection
        }
        
        // Footstep sound logic:
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > moveThreshold)
        {
            if (!footstepAudio.isPlaying)
                footstepAudio.Play();
        }
        else
        {
            if (footstepAudio.isPlaying)
                footstepAudio.Pause();  // Or Stop()
        }

        lastPosition = transform.position;
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

    void OnTriggerStay(Collider other)
    {
        // When staying near a coin
        CoinBehaviour coin = other.GetComponent<CoinBehaviour>();
        if (coin != null)
        {
            nearbyCoin = coin;
            coin.Highlight();
        }
    }

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
}
