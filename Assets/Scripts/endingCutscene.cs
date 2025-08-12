using UnityEngine;
using UnityEngine.Video;

// This script handles the ending cutscene logic.

public class endingCutscene : MonoBehaviour
{
    public GameObject endingMenu; // Reference to the ending menu
    public GameObject player; // Reference to the player object
    public GameObject videoDisplay; // UI object to display video
    public VideoPlayer videoPlayer; // Video player component
    public AudioClip clickSound; // Sound to play on button clicks
    private AudioSource audioSource;  // AudioSource component used to play sounds


    public void StartCutscene()
    {
        // Ensure video player is active
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play(); // Play video
        }
        player.SetActive(false);
        Invoke("EndCutscene", (float)videoPlayer.clip.length);
    }

    private void EndCutscene()
    {
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false); // Hide video

        // End the cutscene
        videoDisplay.SetActive(false);
        endingMenu.SetActive(true);
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
}
