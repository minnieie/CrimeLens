using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Objectives for This Scene")]
    [TextArea] public string[] objectives;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || QuestTracker.Instance == null) return;

        string sceneName = SceneManager.GetActiveScene().name;
        int currentStage = QuestTracker.Instance.GetQuestStage(sceneName);

        // Only set objectives if the scene stage is 0 (first time)
        if (currentStage == 0)
        {
            QuestTracker.Instance.SetQuest(sceneName, currentStage, objectives);

            Debug.Log($"[QuestTrigger] Quest objectives set for {sceneName}, Stage {currentStage}");
        }
        else
        {
            Debug.Log($"[QuestTrigger] Quest already set for {sceneName}, Stage {currentStage}, skipping reset.");
        }
    }
}
