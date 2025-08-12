using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaptchaCode : MonoBehaviour
{
    /// <summary>
    /// This script's main purpose are to generate a code for the player to copy
    /// and verify it.
    /// </summary>
    [SerializeField] private PlayerBehaviour playerBehaviour; // Reference to the player behaviour script
    [SerializeField] TextMeshProUGUI codeText; // UI Text to display the generated code
    [SerializeField] TMP_InputField inputField; // Input field for user to enter the code
    [SerializeField] TextMeshProUGUI feedbackText; // Text to display feedback messages
    [SerializeField] GameObject captchaPanel; // Panel to show the captcha UI
    [SerializeField] Button confirmButton; // Button to verify the entered code
    [SerializeField] int codeLength = 5;
    [SerializeField] float closeDelay = 2f; // Delay before closing the captcha UI after successful verification
    string generatedCode;

    // Verifies the user input against the generated captcha code.
    public void VerifyCode()
    {
        string userInput = inputField.text.ToUpper();

        if (userInput == generatedCode)
        {
            feedbackText.text = "Correct!";
            QuestTracker.Instance.CompleteObjective(1); // Assuming the first objective is the one to complete
            StartCoroutine(CloseAfterDelay()); // Hide the captcha UI after a delay on successful verification
        }
        else
        {
            feedbackText.text = "Incorrect. Try again";
            generatedCode = GenerateCode(codeLength); // Generate a new code
            codeText.text = generatedCode;
            inputField.text = ""; // Clear the input field
        }
    }

    // Generates a random alphanumeric code of the specified length.
    string GenerateCode(int length)
    {
        const string chars = "ABCDFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] code = new char[length];

        for (int i = 0; i < length; i++)
        {
            code[i] = chars[Random.Range(0, chars.Length)];
        }

        return new string(code);
    }

    // Closes the captcha UI after a delay.
    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        HideCaptchaUI(); // Hide the captcha UI after the specified delay
    }

    // Shows the captcha UI.
    public void ShowCaptchaUI()
    {
        StartCoroutine(ShowCaptchaUIDelayed());
    }

    private IEnumerator ShowCaptchaUIDelayed()
    {
        // Ensure parent UI is active before showing captcha
        if (captchaPanel.transform.parent != null)
            captchaPanel.transform.parent.gameObject.SetActive(true);

        yield return null; // Wait 1 frame

        generatedCode = GenerateCode(codeLength);
        codeText.text = generatedCode;
        inputField.text = "";
        feedbackText.text = "";

        captchaPanel.SetActive(true);
    }

    // Hides the captcha UI.
    void HideCaptchaUI()
    {
        // Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        // Cursor.visible = false; // Hide the cursor
        if (captchaPanel != null)
        {
            captchaPanel.SetActive(false); // Hide the captcha panel
            // playerBehaviour.isUILocked = false; // Enable player interaction when captcha is hidden
        }
    }
}
