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

        string sceneName = SceneManager.GetActiveScene().name;
        int stage = QuestTracker.Instance.GetQuestStage(sceneName);

        Debug.Log($"[LobbyQuestTrigger] Lobby stage {stage} detected");

        // Show correct objectives based on stage
        if (stage == 0)
            QuestTracker.Instance.SetQuest(stage0Objectives);
        else if (stage == 1)
            QuestTracker.Instance.SetQuest(stage1Objectives);
    }
}

