using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// This script handles scene transitions when the player interacts with a trigger zone.
// It also displays UI prompts and room names when the player approaches a door or transition point.

public class LevelChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    public int targetSceneIndex; // The build index of the scene to load when the player interacts

    [Header("UI Prompt")]
    public TextMeshProUGUI interactPrompt; // UI text shown when the player is in range to interact

    [Header("Room Display")]
    public TextMeshProUGUI roomNameText;  // UI text element to display the name of the room
    public string roomName;               // Name of the room, set individually for each door/trigger

    private bool isPlayerInRange = false; // Tracks whether the player is within the trigger zone

    [Header("Spawn ID")]
    public string spawnID; // Identifier for the spawn point, used to determine where the player should start in the new scene
    keypadBehaviour lockedDoor;

    void Start()
    {
        // Hide interaction prompt and room name display at the start
        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);

        if (roomNameText != null)
            roomNameText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Get current and target scene names once per frame
        string currentScene = SceneManager.GetActiveScene().name;
        string nextScene = SceneManager.GetSceneByBuildIndex(targetSceneIndex).name;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.instance.lastSceneName = currentScene;
            GameManager.instance.nextSceneName = nextScene;

            if (currentScene == "lobby")
            {
                // Save the player's current position in the lobby, NOT the door's
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    GameManager.instance.returnSpawnPosition = player.transform.position;
                    GameManager.instance.useReturnSpawn = true;
                    Debug.Log("Saved lobby player position: " + GameManager.instance.returnSpawnPosition);
                }
                else
                {
                    Debug.LogWarning("Player object not found to save position!");
                }
            }

            SceneManager.LoadScene(targetSceneIndex); // Load next scene
        }
    }


    void OnTriggerEnter(Collider other)
    {
        // When the player enters the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (lockedDoor == true)
            {
                // Show locked door message
                if (interactPrompt != null)
                    interactPrompt.text = "The door is locked.";
                enabled = false; // Disable this LevelChanger script while the door is locked
            }

            // Show interaction prompt
            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(true);

            // Show room name permanently while player is in range
            if (roomNameText != null)
            {
                roomNameText.text = roomName;
                roomNameText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // When the player exits the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Hide interaction prompt
            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(false);

            // Hide room name immediately
            if (roomNameText != null)
                roomNameText.gameObject.SetActive(false);
        }
    }

    // Removed ShowRoomName coroutine because it's no longer needed
}
