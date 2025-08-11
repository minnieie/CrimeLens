using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Info")]
    public string questTitle;
    [TextArea] public string[] objectives;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (QuestTracker.Instance != null)
            {
                Debug.Log("Quest Triggered: " + questTitle);
                QuestTracker.Instance.SetQuest(questTitle, objectives);
            }
        }
    }
}

