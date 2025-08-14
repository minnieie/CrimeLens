using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyQuestTrigger : MonoBehaviour
{
    [TextArea] public string[] stage0Objectives; // First quest set
    [TextArea] public string[] stage1Objectives; // Second quest set

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int stage = QuestTracker.Instance.GetQuestStage(sceneName);

        if (stage == 0)
        {
            QuestTracker.Instance.SetQuest(stage0Objectives);
        }
        else if (stage == 1)
        {
            QuestTracker.Instance.SetQuest(stage1Objectives);
        }
    }
}

