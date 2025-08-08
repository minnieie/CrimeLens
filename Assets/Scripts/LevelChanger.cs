using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    public int targetSceneIndex;

    [Header("UI Prompt")]
    public TextMeshProUGUI interactPrompt;

    private bool isPlayerInRange = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(targetSceneIndex);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(false);
        }
    }
}
