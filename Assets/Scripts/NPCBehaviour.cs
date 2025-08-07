using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

// This script handles the behavior of NPCs, including dialogue interaction
// and displaying dialogue text on the screen.

public class NPCBehaviour : MonoBehaviour
{
    public string[] dialogueLines;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public float wordDelay = 0.3f;
    public static bool dialogueActive = false;
    private int currentLine = 0;
    public static NPCBehaviour ActiveNPC = null;

    public void StartDialogue()
    {
        if (dialogueActive && dialogueLines.Length > 0)
        {
            nameText.text = gameObject.name;
            dialogueText.text = "";
            // Activate the dialogue UI
            dialogueText.transform.parent.gameObject.SetActive(true);
            ActiveNPC = this;
            Debug.Log("Started interaction with " + gameObject.name);
            dialogueActive = true;
            currentLine = 0;
            StartCoroutine(ShowDialogueLine(dialogueLines[currentLine]));
        }
        else
        {
            StopDialogue();
            dialogueActive = false;
            ActiveNPC = null;
            dialogueText.transform.parent.gameObject.SetActive(false);
            Debug.LogWarning("Dialogue is already active or no dialogue lines are set for " + gameObject.name);
        }
    }

    IEnumerator ShowDialogueLine(string line)
    {
        dialogueText.text = "";
        string[] words = line.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            dialogueText.text += words[i] + " ";
            yield return new WaitForSeconds(wordDelay);
        }
        yield return new WaitForSeconds(1f);
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(ShowDialogueLine(dialogueLines[currentLine]));
        }
        else
        {
            StopDialogue();
        }
    }

    public void StopDialogue()
    {
        dialogueActive = false;
        ActiveNPC = null;
        dialogueText.transform.parent.gameObject.SetActive(false);
        StopAllCoroutines();
        dialogueText.text = "";
        Debug.Log("Dialogue ended with " + gameObject.name);
    }
}