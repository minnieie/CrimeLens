using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Info for Stage 0")]
    [TextArea] public string[] stage0Objectives;

    [Header("Quest Info for Stage 1")]
    [TextArea] public string[] stage1Objectives;

    void Start()
    {
        if (QuestTracker.Instance != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            int stage = QuestTracker.Instance.GetQuestStage(sceneName);

            Debug.Log($"Quest Triggered in {sceneName}, Stage {stage}");

            if (stage == 0)
                QuestTracker.Instance.SetQuest(stage0Objectives);
            else if (stage == 1)
                QuestTracker.Instance.SetQuest(stage1Objectives);
        }
    }

    public class RoomExitTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Change lobby quest stage to 1
                QuestTracker.Instance.SetQuestStage("lobby", 1);
            }
        }
    }
}
