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

    private void Start()
    {
        InitializeQuiz();
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartQuiz);
    }

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


    public void ShowQuestion(int index)
    {
        if (index >= questions.Count)
        {
            EndQuiz();
            return;
        }

        QuizQuestion currentQuestion = questions[index];
        questionText.text = currentQuestion.questionText;

        // Handle video questions
        if (videoPlayer != null)
        {
            bool isVideoQuestion = currentQuestion.type == QuizQuestion.QuestionType.Video && 
                                  currentQuestion.questionVideo != null;

            videoPlayer.gameObject.SetActive(isVideoQuestion);

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

    public void OnAnswerSelected(int answerIndex)
    {   
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

    private void ShowFeedback(bool isCorrect)
    {
        correctFeedback.SetActive(isCorrect);
        wrongFeedback.SetActive(!isCorrect);
        questionText.gameObject.SetActive(false);
    }

    private void MoveToNextQuestion()
    {
        correctFeedback.SetActive(false);
        wrongFeedback.SetActive(false);
        isWaitingForNextQuestion = false;
        questionText.gameObject.SetActive(true);
        currentQuestionIndex++;
        ShowQuestion(currentQuestionIndex);
    }
    private void EndQuiz()
    {
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false);

        quizCompletePanel.SetActive(true);
        questionText.gameObject.SetActive(false);

        foreach (var button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        scoreText.text = $"You answered {correctAnswersCount} out of {questions.Count} questions correctly!";
    }


    public void RestartQuiz()
    {
        correctAnswersCount = 0;

        foreach (var question in questions)
        {
            question.alreadyAnsweredCorrectly = false; // allow replay
            // DO NOT reset hasBeenScored
        }

        quizCompletePanel.SetActive(false);
        questionText.gameObject.SetActive(true);

        scoreText.text = "";

        InitializeQuiz();
    }


}