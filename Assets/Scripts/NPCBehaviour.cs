using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

// This script handles the behavior of NPCs, including dialogue interaction
// and displaying dialogue text on the screen.

public class NPCBehaviour : MonoBehaviour
{
    public class QuizQuestion
    {
        public enum QuestionType { Text, Video } // Type of question: text or video
        public QuestionType type; // Current question type
        public string questionText; // Text of the question
        public List<string> answers; // List of possible answers
        public int correctAnswerIndex; // Index of the correct answer
        public Sprite questionImage; // Optional image for the question

        public bool alreadyAnsweredCorrectly = false; // Tracks if question was answered correctly
        public bool hasBeenScored = false; // Tracks if score was already added for this question
    }

    [Header("Quiz Settings")]
    public List<QuizQuestion> questions; // List of all quiz questions
    public float feedbackDisplayTime = 2f; // Time to show feedback before moving on

    [Header("Startup Settings")]
    public bool startImmediately = true; // Set to false for kiosk mode
    public GameObject quizPanel; // Assign your quiz UI container

    [Header("Audio Settings")]
    public AudioSource audioSource; // Audio source for playing sounds
    public AudioClip correctAnswerSound; // Sound for correct answer
    public AudioClip wrongAnswerSound; // Sound for wrong answer

    [Header("UI References")]
    public TextMeshProUGUI questionText; // UI text for displaying question
    public TextMeshProUGUI scoreText; // UI text for displaying score
    public Button restartButton; // Button to restart the quiz
    public Image questionUIImage; // UI image for displaying question image

    [Header("Scoring")]
    public int pointsPerCorrectAnswer = 10; // Points awarded per correct answer

    public List<Button> answerButtons; // Buttons for selecting answers
    public GameObject correctFeedback; // UI feedback for correct answer
    public GameObject wrongFeedback; // UI feedback for wrong answer
    public GameObject quizCompletePanel; // Panel shown when quiz is complete

    private int correctAnswersCount = 0; // Total number of correct answers
    private int currentQuestionIndex = 0; // Index of current question
    private bool isWaitingForNextQuestion = false; // Prevents multiple answers during feedback
    public string[] dialogueLines; // Array of dialogue lines for the NPC
    public TextMeshProUGUI dialogueText; // Text component to display dialogue
    public TextMeshProUGUI nameText; // Text component to display NPC name
    public float wordDelay = 0.3f; // Delay between words
    public static bool dialogueActive = false; // Flag to check if dialogue is active
    private int currentLine = 0; // Index of the current dialogue line
    public static NPCBehaviour ActiveNPC = null; // Reference to the active NPC
    // Added: Audio support
    public AudioClip talkingClip;   // Assign in Inspector

    // Added: UI prompt for interaction
    public TextMeshProUGUI interactPrompt; // Assign in Inspector
    GameManager score;
    int totalScore = 0;
    int currentScore = 0;


    // Start the dialogue with the NPC
    public void StartDialogue()
    {
        if (dialogueActive && dialogueLines.Length > 0)
        {
            GameManager.instance.questTrackerUI.SetActive(false);
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
        GameManager.instance.questTrackerUI.SetActive(false);
        QuestTracker.Instance.CompleteObjective(1);
    }
    public void AnswerQuestion(int answerIndex)
    {
        if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count)
        {
            Debug.LogError("Invalid question index");
            return;
        }

        // Check if the answer is correct
        if (answerIndex == questions[currentQuestionIndex].correctAnswerIndex)
        {
            Debug.Log("Correct answer!");
            if (score != null)
            {
                score.ModifyScore(pointsPerCorrectAnswer); // Assuming GameManager has an AddPoints(int points) method
            }
        }
        else
        {
            Debug.Log("Incorrect answer.");
        }

        // Proceed to the next question or end the quiz
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            EndQuiz();
        }
    }

    IEnumerator ShowQuestion(int questionIndex)
    {
        if (questionIndex < 0 || questionIndex >= questions.Count)
        {
            Debug.LogError("Invalid question index");
            yield break;
        }

        QuizQuestion currentQuestion = questions[questionIndex];
        questionText.text = currentQuestion.questionText; // Set question text
        // Display the question text
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < currentQuestion.answers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];

                answerButtons[i].onClick.RemoveAllListeners(); // Clear previous listeners
                int buttonIndex = i; // Capture index for closure
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex)); // Add new listener
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false); // Hide unused buttons
            }
        }

        // Wait for player to answer
        while (!answerButtons.Any(button => button.interactable))
        {
            yield return null;
        }

        // Proceed to the next question or end the quiz
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            EndQuiz();
        }
    }

    IEnumerator EndQuiz()
    {
        yield return GetCurrentScore();
        // Show final score
        dialogueText.text = "Quiz completed! Your score: " + totalScore;
    }

    IEnumerator GetCurrentScore()
    {
        yield return new WaitForSeconds(1f); // Simulate some delay
        Debug.Log("Current score: " + currentScore);
        totalScore += currentScore; // Add to total score
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

    // Added: Method to handle answer selection from button
    private void OnAnswerSelected(int buttonIndex)
    {
        AnswerQuestion(buttonIndex);
    }
}