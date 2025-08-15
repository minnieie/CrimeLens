using UnityEngine;
using TMPro;
using System.Collections;

// This script handles the behavior of NPCs, including dialogue interaction
// and displaying dialogue text on the screen.

public class NPCBehaviour : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public string[] dialogueLines; // Array of dialogue lines for the NPC
    public TextMeshProUGUI dialogueText; // Text component to display dialogue
    public TextMeshProUGUI nameText; // Text component to display NPC name
    public float wordDelay = 0.3f; // Delay between words
    public static bool dialogueActive = false; // Flag to check if dialogue is active
    private int currentLine = 0; // Index of the current dialogue line
    public static NPCBehaviour ActiveNPC = null; // Reference to the active NPC
    // Added: Audio support
    public AudioClip talkingClip;   // Assign in Inspector
    public AudioSource audioSource; // Audio source for playing sounds

    // Added: UI prompt for interaction
    public TextMeshProUGUI interactPrompt; // Assign in Inspector


    // Start the dialogue with the NPC
    public void StartDialogue()
    {
        if (dialogueActive && dialogueLines.Length > 0)
        {
            GameManager.instance.questTrackerUI.SetActive(false);
            interactPrompt.gameObject.SetActive(false); // Hide interaction prompt
            // Set the NPC name and dialogue text
            nameText.text = gameObject.name; // Set the NPC name
            dialogueText.text = "";
            // Activate the dialogue UI
            dialogueText.transform.parent.gameObject.SetActive(true);
            ActiveNPC = this; // Set the active NPC
            Debug.Log("Started interaction with " + gameObject.name);
            dialogueActive = true;
            currentLine = 0;

            // Start looping sound
            if (audioSource != null && talkingClip != null)
            {
                audioSource.clip = talkingClip;
                audioSource.loop = true;
                audioSource.Play();
            }

            StartCoroutine(ShowDialogueLine(dialogueLines[currentLine])); // Start showing the first line of dialogue
        }
        else
        {
            StopDialogue(); // Stop any ongoing dialogue
            dialogueActive = false;
            ActiveNPC = null;
            dialogueText.transform.parent.gameObject.SetActive(false); // Deactivate the dialogue UI
            Debug.LogWarning("Dialogue is already active or no dialogue lines are set for " + gameObject.name); // Log a warning
        }
    }

    IEnumerator ShowDialogueLine(string line)
    {
        dialogueText.text = "";
        string[] words = line.Split(' '); // Split the line into individual words
        for (int i = 0; i < words.Length; i++) // Iterate through each word
        {
            dialogueText.text += words[i] + " ";
            yield return new WaitForSeconds(wordDelay); // Wait for the specified delay
        }
        yield return new WaitForSeconds(1f); // Wait before showing the next line
        currentLine++; // Move to the next line
        if (currentLine < dialogueLines.Length) // Check if there are more lines
        {
            StartCoroutine(ShowDialogueLine(dialogueLines[currentLine])); // Start showing the next line of dialogue
        }
        else
        {
            StopDialogue(); // Stop any ongoing dialogue
        }
    }

    public void StopDialogue()
    {
        dialogueActive = false;
        ActiveNPC = null;
        dialogueText.transform.parent.gameObject.SetActive(false); // Deactivate the dialogue UI
        StopAllCoroutines(); // Stop all ongoing coroutines
        dialogueText.text = ""; // Clear the dialogue text

        // Stop looping sound
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        Debug.Log("Dialogue ended with " + gameObject.name); // Log the end of dialogue
        GameManager.instance.questTrackerUI.SetActive(true); // Show quest tracker UI
        interactPrompt.gameObject.SetActive(true); // Show interaction prompt
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int stage = QuestTracker.Instance.GetQuestStage(sceneName);
        int objectiveIndex = 0; // Set the appropriate objective index

        QuestTracker.Instance.CompleteObjective(sceneName, stage, objectiveIndex);
    }

    // Show interaction prompt when player is nearby
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactPrompt != null)
        {
            interactPrompt.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && interactPrompt != null)
        {
            interactPrompt.gameObject.SetActive(false);
        }
    }
}