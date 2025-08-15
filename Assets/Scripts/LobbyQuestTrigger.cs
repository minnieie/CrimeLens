using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyQuestTrigger : MonoBehaviour
{
    [Header("Objectives for Each Stage")]
    public string[] stage0Objectives; // initial lobby quest
    public string[] stage1Objectives; // updated quest after ServerRoom

    private void Start()
    {
        if (QuestTracker.Instance == null) return;

        // Get current scene name and stage
        string sceneName = SceneManager.GetActiveScene().name;
        int stage = QuestTracker.Instance.GetQuestStage(sceneName);

        Debug.Log($"[LobbyQuestTrigger] Lobby stage {stage} detected");

        // Choose objectives based on current stage
        string[] objectivesToSet = null;
        if (stage == 0)
            objectivesToSet = stage0Objectives;
        else if (stage == 1)
            objectivesToSet = stage1Objectives;

        // Set objectives using the new method
        if (objectivesToSet != null)
            QuestTracker.Instance.SetQuest(sceneName, stage, objectivesToSet);
    }
}


