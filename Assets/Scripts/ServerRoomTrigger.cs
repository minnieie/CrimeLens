using UnityEngine;

public class ServerRoomExitTrigger : MonoBehaviour
{
    [Header("Target Scene to Update")]
    public string targetScene = "lobby"; // the lobby scene

    [Header("Stage to Set When Player Leaves")]
    public int newStage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || QuestTracker.Instance == null) return;

        // Update the lobby stage
        QuestTracker.Instance.SetQuestStage(targetScene, newStage);
        Debug.Log($"[ServerRoomExitTrigger] Set {targetScene} stage to {newStage}");
    }
}
