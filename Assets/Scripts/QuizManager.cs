using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using TMPro;


public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public enum QuestionType { Text, Video }
        public QuestionType type;
        public string questionText;
        public List<string> answers;
        public int correctAnswerIndex;
        public VideoClip questionVideo;
        public Sprite questionImage;

        // Track if the question has been answered correctly
        public bool alreadyAnsweredCorrectly = false;
        // Track if the question has been scored
        public bool hasBeenScored = false;

    }

    [Header("Quiz Settings")]
    public List<QuizQuestion> questions;
    public float feedbackDisplayTime = 2f;
    [Header("Video Display")]
    public GameObject videoDisplay; 

    [Header("Audio Settings")]
    public AudioSource audioSource; 
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;

    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI scoreText;
    public Button restartButton;

    [Header("Scoring")]
    public int pointsPerCorrectAnswer = 10;

    public List<Button> answerButtons;
    public GameObject correctFeedback;
    public GameObject wrongFeedback;
    public GameObject quizCompletePanel;
    public VideoPlayer videoPlayer;
    private int correctAnswersCount = 0;
    private int currentQuestionIndex = 0;
    private bool isWaitingForNextQuestion = false;

    // Reference to the player camera and shared zoom camera
    private void Start()
    {
        InitializeQuiz();
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartQuiz);
    }

    // Initialize the quiz by resetting the question index and hiding feedback panels
    public void InitializeQuiz()
    {
        currentQuestionIndex = 0;

        // Make sure videoPlayer is active before showing question
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
        }

        ShowQuestion(currentQuestionIndex);

        correctFeedback.SetActive(false);
        wrongFeedback.SetActive(false);
        quizCompletePanel.SetActive(false);
    }

    // Show the current question based on the index
    public void ShowQuestion(int index)
    {   
        // Check if the index is valid
        if (index >= questions.Count)
        {
            EndQuiz();
            return;
        }

        // Set the current question text
        QuizQuestion currentQuestion = questions[index];
        questionText.text = currentQuestion.questionText;

        // Handle video questions
        if (videoPlayer != null)
        {
            // Check if the question is a video type and has a video clip
            bool isVideoQuestion = currentQuestion.type == QuizQuestion.QuestionType.Video && currentQuestion.questionVideo != null;

            videoPlayer.gameObject.SetActive(isVideoQuestion);

            // If it's a video question, set the video clip and play it
            if (isVideoQuestion)
            {
                videoPlayer.clip = currentQuestion.questionVideo;
                videoPlayer.Play();
            }
        }

        // Set up answer buttons
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < currentQuestion.answers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];
                
                // Remove previous listeners and add new one
                answerButtons[i].onClick.RemoveAllListeners();
                int buttonIndex = i; // Capture index for closure
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Handle answer selection
    public void OnAnswerSelected(int answerIndex)
    {   
        // Prevent multiple answers being selected while waiting for feedback
        if (isWaitingForNextQuestion) return;

        QuizQuestion currentQuestion = questions[currentQuestionIndex];
        bool isCorrect = (answerIndex == currentQuestion.correctAnswerIndex);

        if (isCorrect && !currentQuestion.alreadyAnsweredCorrectly)
        {
            currentQuestion.alreadyAnsweredCorrectly = true;
            correctAnswersCount++;

            if (!currentQuestion.hasBeenScored)
            {
                GameManager.instance.ModifyScore(pointsPerCorrectAnswer);
                currentQuestion.hasBeenScored = true;
            }
        }


        ShowFeedback(isCorrect);
        isWaitingForNextQuestion = true;
        Invoke("MoveToNextQuestion", feedbackDisplayTime);
    }

    // Show feedback based on whether the answer was correct or wrong
    private void ShowFeedback(bool isCorrect)
    {   
        // Hide the video player if it exists
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false);

        correctFeedback.SetActive(isCorrect);
        wrongFeedback.SetActive(!isCorrect);
        questionText.gameObject.SetActive(false);

        // Play the appropriate sound
        if (isCorrect && correctAnswerSound != null)
        {
            audioSource.PlayOneShot(correctAnswerSound);
        }
        else if (!isCorrect && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

    // Move to the next question after feedback is displayed
    private void MoveToNextQuestion()
    {
        correctFeedback.SetActive(false);
        wrongFeedback.SetActive(false);
        isWaitingForNextQuestion = false;
        currentQuestionIndex++;
        ShowQuestion(currentQuestionIndex);
    }

    // End the quiz and display the results
    private void EndQuiz()
    {   
        // Hide the video player if it exists
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false);

        quizCompletePanel.SetActive(true);
        questionText.gameObject.SetActive(false);

        // Hide all answer buttons
        foreach (var button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Display the final score
        scoreText.text = $"You answered {correctAnswersCount} out of {questions.Count} questions correctly!";
    }

    // Restart the quiz, resetting the state and allowing replay of questions
    public void RestartQuiz()
    {
        correctAnswersCount = 0;

        foreach (var question in questions)
        {
            question.alreadyAnsweredCorrectly = false; // allow replay
            // DO NOT reset hasBeenScored
        }

        // Reset the current question indexS
        quizCompletePanel.SetActive(false);
        questionText.gameObject.SetActive(true);
        // Hide feedback panels
        scoreText.text = "";
        // Reset the video player
        InitializeQuiz();
    }


}