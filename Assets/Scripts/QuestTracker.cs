using UnityEngine;
using TMPro;

public class QuestTracker : MonoBehaviour
{
    public static QuestTracker Instance;  // Singleton instance

    [Header("Optional: Assign in Inspector or leave empty to auto-find")]
    public TMP_Text[] objectiveTMPs;

    private bool[] completedObjectives;

    private void Awake()
    {
        // Setup singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        // Auto-find objective TMPs if not assigned or empty
        if (objectiveTMPs == null || objectiveTMPs.Length == 0)
        {
            // Assuming objective TMPs are siblings or children named "Objective1", "Objective2", etc.
            objectiveTMPs = GetComponentsInChildren<TMP_Text>(true);

            if (objectiveTMPs.Length == 0)
            {
                Debug.LogWarning("[QuestTracker] No objective TMP texts found!");
            }
            else
            {
                Debug.Log("[QuestTracker] Found " + objectiveTMPs.Length + " objective TMP texts automatically.");
            }
        }
    }

    // Method to set the current quest objectives
    public void SetQuest(string[] objectives)
    {
        if (objectiveTMPs == null || objectiveTMPs.Length == 0)
        {
            Debug.LogError("[QuestTracker] Cannot set objectives because objectiveTMPs array is empty.");
            return;
        }

        Debug.Log("[QuestTracker] Setting quest: ");

        completedObjectives = new bool[objectives.Length];

        for (int i = 0; i < objectiveTMPs.Length; i++)
        {
            if (i < objectives.Length)
            {
                objectiveTMPs[i].text = "[ ] " + objectives[i];
            }
            else
            {
                objectiveTMPs[i].text = "";
            }
        }
    }

    public void CompleteObjective(int index)
    {
        if (completedObjectives == null || index < 0 || index >= completedObjectives.Length)
        {
            Debug.LogWarning("[QuestTracker] Invalid objective index: " + index);
            return;
        }

        if (completedObjectives[index])
            return; // already completed

        completedObjectives[index] = true;

        if (objectiveTMPs != null && index < objectiveTMPs.Length)
        {
            string currentText = objectiveTMPs[index].text;
            if (currentText.StartsWith("[ ] "))
            {
                objectiveTMPs[index].text = "[X] " + currentText.Substring(4);
            }
        }
    }

    public void ShowTracker()
    {
        gameObject.SetActive(true);
    }

    public void HideTracker()
    {
        gameObject.SetActive(false);
    }
}
