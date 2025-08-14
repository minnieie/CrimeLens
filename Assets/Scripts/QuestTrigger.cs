using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Info")]
    [TextArea] public string[] objectives;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || QuestTracker.Instance == null) return;

        Debug.Log("Quest Triggered: ");
        QuestTracker.Instance.SetQuest(objectives);
        
    }
}


