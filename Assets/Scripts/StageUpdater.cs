using UnityEngine;

public class StageUpdater : MonoBehaviour
{
    [Header("Scene to Update")]
    public string sceneToUpdate; // e.g., "lobby"

    [Header("Stage to Set When Player Enters Trigger")]
    public int stageToSet = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || QuestTracker.Instance == null) return;

        int currentStage = QuestTracker.Instance.GetQuestStage(sceneToUpdate);

        if (currentStage < stageToSet)
        {
            QuestTracker.Instance.SetQuestStage(sceneToUpdate, stageToSet);
            Debug.Log($"[StageUpdaterTrigger] Set {sceneToUpdate} stage to {stageToSet}");
        }
        else
        {
            Debug.Log($"[StageUpdaterTrigger] {sceneToUpdate} stage already {currentStage}, skipping.");
        }
    }
}
