using System.Collections.Generic;
using UnityEngine;

// This script manages the UI flow for a kiosk application, including welcome screen, quiz, and informational read-up pages
public class KioskUIManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject welcomePanel; // The initial welcome screen shown to users
    public GameObject quizPanel;    // The panel that displays the quiz interface

    [Header("Read-Up Pages")]
    public List<GameObject> readUpPages; // List of informational pages users can read before taking the quiz
    private int currentPage = 0;         // Tracks the currently displayed read-up page

    [Header("Quiz Manager")]
    public QuizManager quizManager; // Reference to the QuizManager that handles quiz logic

    private bool quizHasStarted = false; // Ensures the quiz is only initialized once

    // Called when "Start Quiz" button is pressed
    public void OnStartQuizPressed()
    {
        welcomePanel.SetActive(false); // Hide welcome screen
        quizPanel.SetActive(true);     // Show quiz panel

        // Show score text if available
        if (quizManager != null && quizManager.scoreText != null)
        {
            quizManager.scoreText.gameObject.SetActive(true);
        }

        // Initialize quiz only once
        if (!quizHasStarted)
        {
            quizManager.InitializeQuiz();
            quizHasStarted = true;
        }
    }

    // Called when "Read Up" button is pressed
    public void OnReadUpPressed()
    {
        Debug.Log("Read Up button pressed");
        welcomePanel.SetActive(false); // Hide welcome screen
        quizPanel.SetActive(false);    // Hide quiz panel

        // Hide score text if available
        if (quizManager != null && quizManager.scoreText != null)
        {
            quizManager.scoreText.gameObject.SetActive(false);
        }

        ShowReadUpPage(0); // Show the first read-up page
    }

    // Displays the specified read-up page and hides others
    private void ShowReadUpPage(int pageIndex)
    {
        for (int i = 0; i < readUpPages.Count; i++)
        {
            readUpPages[i].SetActive(i == pageIndex); // Only activate the selected page
        }
        currentPage = pageIndex; // Update current page index
    }

    // Called when "Next Page" button is clicked
    public void OnNextPage()
    {
        Debug.Log("Next Page button clicked");
        if (currentPage < readUpPages.Count - 1)
        {
            ShowReadUpPage(currentPage + 1); // Show next page
        }
    }

    // Called when "Previous Page" button is clicked
    public void OnPreviousPage()
    {
        Debug.Log("Previous Page button clicked");
        if (currentPage > 0)
        {
            ShowReadUpPage(currentPage - 1); // Show previous page
        }
    }

    // Returns to the welcome screen from read-up pages
    public void OnBackToWelcome()
    {
        foreach (GameObject page in readUpPages)
        {
            page.SetActive(false); // Hide all read-up pages
        }

        quizPanel.SetActive(false);  // Hide quiz panel
        welcomePanel.SetActive(true); // Show welcome screen
    }

    // Returns to the welcome screen from the quiz panel
    public void OnBackToWelcomeFromQuiz()
    {
        quizPanel.SetActive(false);  // Hide quiz panel
        welcomePanel.SetActive(true); // Show welcome screen
    }
}
