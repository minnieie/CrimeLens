using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private CoinBehaviour nearbyCoin; // The coin the player is near

    void Update()
    {
        // Check for "E" key press and collect the coin if nearby
        if (nearbyCoin != null && Input.GetKeyDown(KeyCode.E))
        {
            nearbyCoin.Collect(this); // Collect coin
            nearbyCoin = null; // Clear reference after collection
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
