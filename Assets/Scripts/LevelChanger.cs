using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    // This field will show up in the Inspector
    public int targetSceneIndex;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Load the scene specified in the Inspector
            SceneManager.LoadScene(targetSceneIndex);
        }
    }
}