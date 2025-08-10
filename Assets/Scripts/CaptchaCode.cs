using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaptchaCode : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour playerBehaviour; // Reference to the player behaviour script
    [SerializeField] TextMeshProUGUI codeText; // UI Text to display the generated code
    [SerializeField] TMP_InputField inputField; // Input field for user to enter the code
    [SerializeField] TextMeshProUGUI feedbackText; // Text to display feedback messages
    [SerializeField] GameObject captchaPanel; // Panel to show the captcha UI
    [SerializeField] Button confirmButton; // Button to verify the entered code
    [SerializeField] int codeLength = 5;
    [SerializeField] float closeDelay = 2f; // Delay before closing the captcha UI after successful verification
    string generatedCode;

    void Start()
    {
        if (captchaPanel != null)
        {
            captchaPanel.SetActive(false); // Hide the captcha panel at the start
        }
    }

    public void VerifyCode()
    {
        string userInput = inputField.text.ToUpper();

        if (userInput == generatedCode)
        {
            feedbackText.text = "Correct!";
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

    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        HideCaptchaUI(); // Hide the captcha UI after the specified delay
    }

    public void ShowCaptchaUI()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible

        if (captchaPanel != null)
        {
            captchaPanel.SetActive(true); // Show the captcha panel
        }
        generatedCode = GenerateCode(codeLength); // Generate a new code
        codeText.text = generatedCode;
        inputField.text = ""; // Clear the input field
        feedbackText.text = ""; // Clear any previous feedback
        playerBehaviour.isUILocked = true; // Disable player interaction while captcha is active
    }

    void HideCaptchaUI()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
        if (captchaPanel != null)
        {
            captchaPanel.SetActive(false); // Hide the captcha panel
            playerBehaviour.isUILocked = false; // Enable player interaction when captcha is hidden
        }
    }
}
