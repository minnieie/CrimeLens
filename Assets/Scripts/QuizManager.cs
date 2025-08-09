using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using TMPro;

// This script manages a multimedia quiz system in Unity.
// It supports both text-based and video-based questions, handles user input,
// provides feedback for correct and incorrect answers, tracks scoring,
// and displays the final results. It also includes functionality to restart the quiz.


// Main class that manages the quiz logic
public class QuizManager : MonoBehaviour
{
    // Represents a single quiz question
    [System.Serializable]
    public class QuizQuestion
    {
        public enum QuestionType { Text, Video } // Type of question: text or video
        public QuestionType type; // Current question type
        public string questionText; // Text of the question
        public List<string> answers; // List of possible answers
        public int correctAnswerIndex; // Index of the correct answer
        public VideoClip questionVideo; // Video clip for video-type questions
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

    [Header("Video Display")]
    public GameObject videoDisplay; // UI object to display video

    [Header("Audio Settings")]
    public AudioSource audioSource; // Audio source for playing sounds
    public AudioClip correctAnswerSound; // Sound for correct answer
    public AudioClip wrongAnswerSound; // Sound for wrong answer

    [Header("UI References")]
    public TextMeshProUGUI questionText; // UI text for displaying question
    public TextMeshProUGUI scoreText; // UI text for displaying score
    public Button restartButton; // Button to restart the quiz
    public Image questionUIImage; 

    [Header("Scoring")]
    public int pointsPerCorrectAnswer = 10; // Points awarded per correct answer

    public List<Button> answerButtons; // Buttons for selecting answers
    public GameObject correctFeedback; // UI feedback for correct answer
    public GameObject wrongFeedback; // UI feedback for wrong answer
    public GameObject quizCompletePanel; // Panel shown when quiz is complete
    public VideoPlayer videoPlayer; // Video player component

    private int correctAnswersCount = 0; // Total number of correct answers
    private int currentQuestionIndex = 0; // Index of current question
    private bool isWaitingForNextQuestion = false; // Prevents multiple answers during feedback

    // Called when the game starts
    private void Start()
    {
        if (startImmediately)
        {
            quizPanel.SetActive(true);
            InitializeQuiz();
        }
        else
        {
            quizPanel.SetActive(false); // Wait for external trigger
        }

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartQuiz);
    }


    // Initializes the quiz state
    public void InitializeQuiz()
    {
        currentQuestionIndex = 0;

        // Ensure video player is active
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
        }

        ShowQuestion(currentQuestionIndex); // Show first question

        // Hide feedback and completion panels
        correctFeedback.SetActive(false);
        wrongFeedback.SetActive(false);
        quizCompletePanel.SetActive(false);
    }

    // Displays the question at the given index
    public void ShowQuestion(int index)
    {
        Debug.Log("Questions count at start: " + questions.Count);
        Debug.Log("Current question index: " + currentQuestionIndex);
        // If index is out of bounds, end the quiz
        if (index >= questions.Count)
        {
            EndQuiz();
            return;
        }

        QuizQuestion currentQuestion = questions[index];
        questionText.text = currentQuestion.questionText; // Set question text

        // Handle video-type questions
        if (videoPlayer != null)
        {
            bool isVideoQuestion = currentQuestion.type == QuizQuestion.QuestionType.Video && currentQuestion.questionVideo != null;
            videoPlayer.gameObject.SetActive(isVideoQuestion);

            if (isVideoQuestion)
            {
                videoPlayer.clip = currentQuestion.questionVideo;
                videoPlayer.Play(); // Play video
            }
        }

        // Set up answer buttons
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
        
        // Show question image if available
        if (questionUIImage != null)
        {
            if (currentQuestion.questionImage != null)
            {
                questionUIImage.sprite = currentQuestion.questionImage;
                questionUIImage.gameObject.SetActive(true);
            }
            else
            {
                questionUIImage.gameObject.SetActive(false); // Hide if no image
            }
        }
    }

    // Called when an answer is selected
    public void OnAnswerSelected(int answerIndex)
    {
        if (isWaitingForNextQuestion) return; // Prevent multiple selections

        QuizQuestion currentQuestion = questions[currentQuestionIndex];
        bool isCorrect = (answerIndex == currentQuestion.correctAnswerIndex);

        // Update score if correct and not already answered
        if (isCorrect && !currentQuestion.alreadyAnsweredCorrectly)
        {
            currentQuestion.alreadyAnsweredCorrectly = true;
            correctAnswersCount++;

            if (!currentQuestion.hasBeenScored)
            {
                GameManager.instance.ModifyScore(pointsPerCorrectAnswer); // Add points
                currentQuestion.hasBeenScored = true;
            }
        }

        ShowFeedback(isCorrect); // Show feedback
        isWaitingForNextQuestion = true;
        Invoke("MoveToNextQuestion", feedbackDisplayTime); // Wait then move on
    }

    // Displays feedback based on correctness of answer
    private void ShowFeedback(bool isCorrect)
    {
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false); // Hide video

        correctFeedback.SetActive(isCorrect); // Show correct feedback
        wrongFeedback.SetActive(!isCorrect); // Show wrong feedback
        questionText.gameObject.SetActive(false); // Hide question text

        // Play sound
        if (isCorrect && correctAnswerSound != null)
        {
            audioSource.PlayOneShot(correctAnswerSound);
        }
        else if (!isCorrect && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

    // Moves to the next question
    private void MoveToNextQuestion()
    {   
        Debug.Log("Moving to next question: " + currentQuestionIndex);

        correctFeedback.SetActive(false);
        wrongFeedback.SetActive(false);
        isWaitingForNextQuestion = false;
        questionText.gameObject.SetActive(true);
        currentQuestionIndex++;
        ShowQuestion(currentQuestionIndex); // Show next question
    }

    // Ends the quiz and shows results
    private void EndQuiz()
    {
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false); // Hide video

        quizCompletePanel.SetActive(true); // Show completion panel
        questionText.gameObject.SetActive(false); // Hide question

        // Hide all answer buttons
        foreach (var button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Show final score
        scoreText.text = $"You answered {correctAnswersCount} out of {questions.Count} questions correctly!";
    }

    // Restarts the quiz
    public void RestartQuiz()
    {
        correctAnswersCount = 0;

        foreach (var question in questions)
        {
            question.alreadyAnsweredCorrectly = false; // Allow replay
            // Do not reset hasBeenScored to preserve scoring integrity
        }

        quizCompletePanel.SetActive(false); // Hide completion panel
        questionText.gameObject.SetActive(true); // Show question text
        scoreText.text = ""; // Clear score text

        InitializeQuiz(); // Reinitialize quiz
    }
}
