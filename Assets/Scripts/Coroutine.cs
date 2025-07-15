using UnityEngine;
using System.Collections;

public class Coroutine : MonoBehaviour
{   
    [SerializeField]
    int[] intsToPrint;

    [SerializeField]
    float pauseDuration;

    [SerializeField]
    bool continueCoroutine = false; // This will control when TaskTwo can proceed
    
    void Start()
    {
        // Start the first coroutine
        StartCoroutine(TaskOne());
    }

    private IEnumerator TaskOne()
    {   
        yield return TaskTwo(); // Wait for TaskTwo to complete before proceeding   
        for (int i = 0; i < intsToPrint.Length; i++)
        {
            Debug.Log("Coroutine iteration: " + i);
            yield return new WaitForSeconds(pauseDuration);
        }
    }
    private IEnumerator TaskTwo()
    {
        while (!continueCoroutine)
        {
            Debug.Log("Waiting for condition to continue...");
            yield return null; // Wait for the next frame
        }
        Debug.Log("continueCoroutine is now true, proceeding with TaskTwo");
    }

}
