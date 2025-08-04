using UnityEngine;

public class BackupSpawner : MonoBehaviour
{
    private void OnEnable()
    {
        Chaser.OnThirdStrike += LogAlert;
    }

    private void OnDisable()
    {
        Chaser.OnThirdStrike -= LogAlert;
    }

    void LogAlert()
    {
        Debug.Log("⚠️ ALERT: Backup should spawn here! (Waiting for Andre to implement)");
    }
}
