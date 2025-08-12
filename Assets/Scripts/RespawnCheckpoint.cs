using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour
{
    private void Start()
    {
        // Find the player in the scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerBehaviour player = playerObject.GetComponent<PlayerBehaviour>();
            if (player != null)
            {
                player.SetRespawnPoint(transform.position);
                Debug.Log("Respawn point set at scene start.");
            }
        }
        else
        {
            Debug.LogWarning("Player not found in the scene.");
        }
    }
}
