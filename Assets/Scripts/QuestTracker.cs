using UnityEngine;
using TMPro;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEngine.SceneManagement;

public class QuestTracker : MonoBehaviour
{
    public static QuestTracker Instance;  // Singleton instance
    // sceneName → stage → completed objectives
    // Tracks current stage per scene
    private Dictionary<string, int> sceneStages = new Dictionary<string, int>();

    // Tracks completed objectives per scene & stage
    private Dictionary<string, Dictionary<int, bool[]>> completedObjectivesByStage = new();

    [Header("Assign in Inspector or leave empty to auto-find")]
    public TMP_Text[] objectiveTMPs;


    // Tracks quest stages per scene (sceneName → stage index)
    private Dictionary<string, int> sceneQuestStages = new Dictionary<string, int>();

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

    // -----------------------------
    // QUEST STAGE MANAGEMENT
    // -----------------------------

    public int GetQuestStage(string scene)
    {
        if (!sceneStages.ContainsKey(scene))
            sceneStages[scene] = 0;
        return sceneStages[scene];
    }

    public void SetQuestStage(string scene, int stage)
    {
        sceneStages[scene] = stage;
        Debug.Log($"[QuestTracker] Quest stage for scene '{scene}' set to {stage}");
    }

    // -----------------------------
    // QUEST OBJECTIVE DISPLAY
    // -----------------------------

    public void SetQuest(string scene, int stage, string[] objectives)
    {
        if (objectiveTMPs == null || objectiveTMPs.Length == 0) return;

        // Get or create the completedObjectives array for this scene & stage
        if (!completedObjectivesByStage.ContainsKey(scene))
            completedObjectivesByStage[scene] = new Dictionary<int, bool[]>();

        if (!completedObjectivesByStage[scene].ContainsKey(stage))
            completedObjectivesByStage[scene][stage] = new bool[objectives.Length];

        bool[] completed = completedObjectivesByStage[scene][stage];

        // Update display
        for (int i = 0; i < objectiveTMPs.Length; i++)
        {
            if (i < objectives.Length)
                objectiveTMPs[i].text = completed[i] ? "[X] " + objectives[i] : "[ ] " + objectives[i];
            else
                objectiveTMPs[i].text = "";
        }
    }


    public void CompleteObjective(string scene, int stage, int index)
    {
        if (!completedObjectivesByStage.ContainsKey(scene) ||
            !completedObjectivesByStage[scene].ContainsKey(stage)) return;

        bool[] completed = completedObjectivesByStage[scene][stage];
        if (index < 0 || index >= completed.Length || completed[index]) return;

        completed[index] = true;

        if (objectiveTMPs != null && index < objectiveTMPs.Length)
        {
            string currentText = objectiveTMPs[index].text;
            if (currentText.StartsWith("[ ] "))
                objectiveTMPs[index].text = "[X] " + currentText.Substring(4);
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

     private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "ServerRoom")
        {
            Debug.Log($"[QuestTracker] Player exited trigger in {currentScene} → Setting lobby stage to 1");
            SetQuestStage("lobby", 1);
        }
    } 
}

