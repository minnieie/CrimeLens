using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

// This script handles the ending cutscene logic.

public class endingCutscene : MonoBehaviour
{
    public GameObject endingMenu; // Reference to the ending menu
    public GameObject player; // Reference to the player object
    public GameObject videoDisplay; // UI object to display video
    public VideoPlayer videoPlayer; // Video player component
    public AudioClip clickSound; // Sound to play on button clicks
    public TextMeshProUGUI scoreText; // Reference to the score text UI element
    public TextMeshProUGUI messageText; // Reference to the message text UI element
    private AudioSource audioSource;  // AudioSource component used to play sounds
    GameManager score;

    public void StartCutscene()
    {
        endingMenu.SetActive(false); // Hide ending menu
        // Ensure video player is active
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
            videoDisplay.SetActive(true); // Show video display
            videoPlayer.Play(); // Play video
        }
        player.SetActive(false);
        // Initialize audio source if not already set
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
        Invoke("EndCutscene", (float)videoPlayer.clip.length);
    }

    private void EndCutscene()
    {
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(false); // Hide video
            videoPlayer.Stop(); // Stop video playback
            videoDisplay.SetActive(false); // Hide video display
        }
        // End the cutscene
        videoDisplay.SetActive(false);
        endingMenu.SetActive(true);
        scoreText.text = "Your Score: " + GameManager.instance.score.ToString();
        messageText.text = "Thank you for playing! You have successfully infiltrated the company and have escaped.\n\nYour score reflects your performance during the game.\n\nYou can try again and attempt to get a higher score.";
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make cursor visible
    }

    void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void ExitGame()
    {
        PlayClickSound(); // Play feedback sound
        Debug.Log("Exit game"); // Log exit action (useful for testing in editor)
        Application.Quit(); // Quit the application (only works in build)
    }

    public void RestartGame()
    {
        PlayClickSound(); // Play feedback sound
        SceneManager.LoadScene("Menu"); // Reload the current scene
        GameManager.instance.score = 0; // Reset score
    }
}
