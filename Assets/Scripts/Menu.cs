using UnityEngine;
using UnityEngine.SceneManagement;

//  This Script Controls the main menu UI and interactions such as starting the game, viewing credits, and exiting
public class MainMenuController : MonoBehaviour
{
    // Reference to the credits panel UI
    public GameObject creditsPanel;

    // Reference to the logo object that will pulse
    public GameObject logo;

    // Controls the pulsing animation speed and intensity
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;

    // Stores the original scale of the logo for pulsing effect
    private Vector3 originalLogoScale;

    [Header("Scene Settings")]
    public int targetSceneIndex;  // Build index of the scene to load when starting the game

    [Header("Audio Settings")]
    public AudioClip clickSound;      // Sound to play on button clicks
    public AudioClip backgroundMusic; // Background music clip
    private AudioSource audioSource;  // AudioSource component used to play sounds

    void Start()
    {
        // Hide credits panel when the menu first loads
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        // Store the original scale of the logo for animation
        if (logo != null)
            originalLogoScale = logo.transform.localScale;

        // Try to get an existing AudioSource, or add one if missing
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setup and play background music
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f; // Optional: adjust volume
            audioSource.Play();
        }
    }

    void Update()
    {
        // Animate the logo with a pulsing effect
        if (logo != null)
        {
            float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            logo.transform.localScale = originalLogoScale * scaleFactor;
        }
    }

    // Plays the click sound effect
    void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Called when "Start Game" button is pressed
    public void StartGame()
    {
        PlayClickSound(); // Play feedback sound
        SceneManager.LoadScene(targetSceneIndex); // Load the target scene
    }

    // Called when "Credits" button is pressed
    public void OpenCredits()
    {
        PlayClickSound(); // Play feedback sound
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true); // Show credits panel
        }
    }

    // Called when "Back" button is pressed in credits panel
    public void CloseCredits()
    {
        PlayClickSound(); // Play feedback sound
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false); // Hide credits panel
        }
    }

    // Called when "Exit" button is pressed
    public void ExitGame()
    {
        PlayClickSound(); // Play feedback sound
        Debug.Log("Exit game"); // Log exit action (useful for testing in editor)
        Application.Quit(); // Quit the application (only works in build)
    }
}
