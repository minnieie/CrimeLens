using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Info")]
    [TextArea] public string[] objectives;

    void Start()
    {
        if (QuestTracker.Instance != null)
        {
                Debug.Log("Quest Triggered: ");
                QuestTracker.Instance.SetQuest(objectives);
            }
        }
    }


