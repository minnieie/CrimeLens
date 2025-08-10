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
            if (QuesTracker.Instance != null)
            {
                Debug.Log("Quest Triggered: " + questTitle);
                QuesTracker.Instance.SetQuest(questTitle, objectives);
            }
        }
    }
}

