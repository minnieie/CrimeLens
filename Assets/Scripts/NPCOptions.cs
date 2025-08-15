using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Manages NPC dialogue interactions with multiple choice options.
/// Displays dialogue lines, NPC name, and handles user input through UI buttons.
/// </summary>
public class NPCOptions : MonoBehaviour
{
    /// <summary>
    /// UI elements for displaying dialogue and NPC name.
    /// </summary>
    [Header("Dialogue Settings")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public GameObject dialoguePanel;
    public GameObject optionsPanel;
    public TextMeshProUGUI interactPrompt;

    /// <summary>
    /// List of buttons used to display dialogue options.
    /// These should be assigned in the Unity Inspector.
    /// </summary>
    [Header("Option Buttons")]
    public List<Button> optionButtons;

    /// <summary>
    /// Represents a single node in the dialogue tree.
    /// Contains the dialogue line, available options, and indices of next nodes.
    /// </summary>
    [System.Serializable]
    public class DialogueNode
    {
        /// <summary>
        /// The dialogue line spoken by the NPC.
        /// </summary>
        public string line;

        /// <summary>
        /// List of options the player can choose from.
        /// </summary>
        public List<string> options;

        /// <summary>
        /// Indices pointing to the next dialogue nodes for each option.
        /// </summary>
        public List<int> nextNodeIndices;

        /// <summary>
        /// Score values associated with each option.
        /// </summary>
        public List<int> optionScores;

        /// <summary>
        /// Flags indicating if the score for this option has already been awarded.
        /// Initialized to false for all options.
        /// </summary>
        [System.NonSerialized]
        public List<bool> hasScoredFlags;
    }

    /// <summary>
    /// The full list of dialogue nodes that make up the conversation.
    /// </summary>
    public List<DialogueNode> dialogueNodes;

    /// <summary>
    /// Tracks the current position in the dialogue tree.
    /// </summary>
    private int currentNodeIndex = 0;
    public static NPCOptions ActiveNPC = null;

    private void Awake()
    {
        // Initialize hasScoredFlags list for all nodes and options
        foreach (var node in dialogueNodes)
        {
            if (node.hasScoredFlags == null || node.hasScoredFlags.Count != node.options.Count)
            {
                node.hasScoredFlags = new List<bool>();
                for (int i = 0; i < node.options.Count; i++)
                {
                    node.hasScoredFlags.Add(false);
                }
            }
        }
    }

    /// <summary>
    /// Begins the dialogue sequence by showing the first node.
    /// Also displays the NPC's name and unlocks the cursor.
    /// </summary>
    public void StartDialogue()
    {
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ActiveNPC = this; // Set the active NPC
        GameManager.instance.questTrackerUI.SetActive(false); // Hide quest tracker
        interactPrompt.gameObject.SetActive(false); // Hide interaction prompt

        nameText.text = gameObject.name;
        dialoguePanel.SetActive(true); // Show dialogue panel
        currentNodeIndex = 0; // Reset current node index
        ShowDialogueNode(dialogueNodes[currentNodeIndex]); // Show the first dialogue node
    }

    /// <summary>
    /// Displays the current dialogue line and its associated options.
    /// </summary>
    /// <param name="node">The dialogue node to display.</param>
    void ShowDialogueNode(DialogueNode node)
    {
        dialogueText.text = node.line;
        StartCoroutine(ShowOptions(node));
    }


    /// <summary>
    /// Displays the available options for the current dialogue node.
    /// Sets up button listeners and updates button text.
    /// </summary>
    /// <param name="node">The dialogue node containing options.</param>
    IEnumerator ShowOptions(DialogueNode node)
    {
        optionsPanel.SetActive(true); // Show options panel
        Debug.Log($"Showing {node.options.Count} options");

        // Disable all buttons and remove old listeners
        foreach (var btn in optionButtons)
        {
            btn.gameObject.SetActive(false);
            btn.onClick.RemoveAllListeners();
        }

        // Enable and setup buttons according to current options
        for (int i = 0; i < node.options.Count; i++)
        {
            if (i >= optionButtons.Count)
            {
                Debug.LogWarning("Not enough buttons assigned in inspector!");
                break;
            }

            Button btn = optionButtons[i]; // Get the button for this option
            btn.gameObject.SetActive(true); // Show the button
            btn.GetComponentInChildren<TextMeshProUGUI>().text = node.options[i]; // Set button text
            int index = i; // capture local copy for closure
            btn.onClick.AddListener(() => // Add listener for button click
            {
                Debug.Log($"Option {node.options[index]} clicked");

                int scoreToAdd = 0;
                bool alreadyScored = false;

                // Check if optionScores and hasScoredFlags are initialized
                if (node.optionScores != null && index < node.optionScores.Count)
                    scoreToAdd = node.optionScores[index];

                // Check if hasScoredFlags is initialized
                if (node.hasScoredFlags != null && index < node.hasScoredFlags.Count)
                    alreadyScored = node.hasScoredFlags[index];

                // Only add score if not scored before and scoreToAdd != 0
                if (!alreadyScored && scoreToAdd != 0 && GameManager.instance != null)
                {
                    GameManager.instance.ModifyScore(scoreToAdd);
                    node.hasScoredFlags[index] = true;
                    Debug.Log($"Added {scoreToAdd} points to GameManager score");
                }
                else if (alreadyScored)
                {
                    Debug.Log($"Option {node.options[index]} already scored, no points added.");
                }

                OnOptionSelected(node.nextNodeIndices[index]);
            });
        }
        yield return new WaitForSeconds(0.1f); // Small delay to ensure UI updates
    }

    /// <summary>
    /// Handles the player's selection of a dialogue option.
    /// Advances to the next node or ends the dialogue if no valid node exists.
    /// </summary>
    /// <param name="nextIndex">Index of the next dialogue node.</param>
    public void OnOptionSelected(int nextIndex)
    {
        optionsPanel.SetActive(false); // Hide options panel
        if (nextIndex >= 0 && nextIndex < dialogueNodes.Count)
        {
            currentNodeIndex = nextIndex;
            ShowDialogueNode(dialogueNodes[currentNodeIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// Ends the dialogue sequence and hides all related UI elements.
    /// Locks and hides the cursor again.
    /// </summary>
    public void EndDialogue()
    {
        GameManager.instance.questTrackerUI.SetActive(true); // Show quest tracker UI
        interactPrompt.gameObject.SetActive(true); // Show interaction prompt
        dialoguePanel.SetActive(false); // Hide dialogue panel
        optionsPanel.SetActive(false); // Hide options panel
        dialogueText.text = ""; // Clear dialogue text

        // Hide cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
