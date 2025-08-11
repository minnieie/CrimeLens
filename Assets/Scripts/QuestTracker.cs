using UnityEngine;
using TMPro;

public class QuestTracker : MonoBehaviour
{
    public static QuestTracker Instance;  // Singleton instance

    [Header("Optional: Assign in Inspector or leave empty to auto-find")]
    public TMP_Text questTitleTMP;
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

        // Auto-find quest title TMP if not assigned
        if (questTitleTMP == null)
        {
            questTitleTMP = GetComponentInChildren<TMP_Text>(true);
            if (questTitleTMP == null)
            {
                Debug.LogWarning("[QuestTracker] Quest Title TMP text NOT found!");
            }
        }

        // Auto-find objective TMPs if not assigned or empty
        if (objectiveTMPs == null || objectiveTMPs.Length == 0)
        {
            // Assuming objective TMPs are siblings or children named "Objective1", "Objective2", etc.
            objectiveTMPs = GetComponentsInChildren<TMP_Text>(true);

            // Filter out the quest title text if it's in the same list
            if (questTitleTMP != null)
            {
                var list = new System.Collections.Generic.List<TMP_Text>(objectiveTMPs);
                list.Remove(questTitleTMP);
                objectiveTMPs = list.ToArray();
            }

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

    public void SetQuest(string title, string[] objectives)
    {
        if (questTitleTMP == null)
        {
            Debug.LogError("[QuestTracker] Cannot set quest title because questTitleTMP is null.");
            return;
        }
        if (objectiveTMPs == null || objectiveTMPs.Length == 0)
        {
            Debug.LogError("[QuestTracker] Cannot set objectives because objectiveTMPs array is empty.");
            return;
        }

        Debug.Log("[QuestTracker] Setting quest: " + title);

        questTitleTMP.text = title;
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
}
