using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;

    [SerializeField]
    TextMeshProUGUI scoreText;

    [Header("Return Spawn Settings")]
    public string lastSceneName;
    public string nextSceneName;
    public string returnSpawnID;
    public bool useReturnSpawn;
    public Vector3 returnSpawnPosition;

    [Header("Quest Tracker")]
    public GameObject questTrackerUI;


    void Awake()
    {
        // This is a LAZY singleton
        // Check if there is instance already and if the instance is the object
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // If it's not, destroy this object
        }
        else
        {
            instance = this; // If the instance is already set, do nothing
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
            Debug.Log("GameManager initialized in scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void ModifyScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
        scoreText.text = "Score: " + score; // Update the score text
    }

    public void TestFunction()
    {
        Debug.Log("Test function called from GameManager");
    }
}