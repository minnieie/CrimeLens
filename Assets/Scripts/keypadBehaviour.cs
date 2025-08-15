using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class keypadBehaviour : MonoBehaviour
{
    [Header("Keypad Settings")]
    public List<Button> keypadButtons; // List of buttons
    public TextMeshProUGUI displayText; // Assign in inspector: shows entered numbers
    // public TextMeshProUGUI messageText; // Assign in inspector: shows error/success messages
    // public GameObject door; // Assign in inspector: the door to unlock
    // public GameObject levelChanger; // Assign in inspector: the level changer object
    public GameObject keypadPanel; // Assign in inspector: the keypad UI panel
    private string enteredCode = "";
    private readonly string correctCode = "2724"; // Set your desired code here
    public bool lockedDoor = true; // Track if the door is locked

    public TextMeshProUGUI interactPrompt;

    private void LockPlayerScreen(bool lockScreen)
    {
        if (lockScreen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Call this when player interacts with keypad (e.g., OnTriggerEnter or button press)
    public void ShowKeypad()
    {
        // Set up keypad buttons
        for (int i = 0; i < keypadButtons.Count; i++)
        {
            if (i < keypadButtons.Count)
            {
                keypadButtons[i].interactable = true; // Ensure button is interactable
                int buttonIndex = i; // Capture index for closure
                keypadButtons[i].onClick.RemoveAllListeners(); // Clear previous listeners
                keypadButtons[i].onClick.AddListener(() => PressNumber(buttonIndex)); // Add new listener
            }
        }
        keypadPanel.SetActive(true);
        displayText.text = "";
        LockPlayerScreen(true); // Lock the player screen when keypad is shown
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    // Call this from each number button (pass the number as string)
    public void PressNumber(int number)
    {
        if (enteredCode.Length < 4)
        {
            enteredCode += number.ToString();
            displayText.text = enteredCode;
        }
        if (enteredCode.Length == 4)
        {
            StartCoroutine(CheckCode());
        }
        if (enteredCode.Length > 4)
        {
            enteredCode = enteredCode.Substring(1);
            displayText.text = enteredCode;
        }
    }

    private IEnumerator CheckCode()
    {
        yield return new WaitForSeconds(0.5f); // Optional delay for better UX
        if (enteredCode == correctCode)
        {
            StartCoroutine(ShowSuccessMessage());
        }
        else
        {
            StartCoroutine(ShowErrorMessage());
        }
    }

    private IEnumerator ShowSuccessMessage()
    {
        displayText.text = "Unlocked!";
        yield return new WaitForSeconds(2f);
        displayText.text = "";
        lockedDoor = false; // Assuming you have a variable to track if the door is locked
        // UnlockDoor();
        keypadPanel.SetActive(false);
        enteredCode = ""; // reset code
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor again
        Cursor.visible = false; // Hide the cursor
    }

    private IEnumerator ShowErrorMessage()
    {
        displayText.text = "Error! Try again";
        yield return new WaitForSeconds(2f);
        displayText.text = "";
        enteredCode = ""; // reset code
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
