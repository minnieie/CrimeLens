using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chaser : MonoBehaviour
{
    NavMeshAgent myAgent;

    [SerializeField]
    Transform targetTransform;

    public string currentState;

    void Awake()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        StartCoroutine(SwitchState("Idle"));
    }

    IEnumerator SwitchState(string newState)
    {
        if (currentState == newState)
        {
            yield break; // Exit if the state is already the same
        }

        // Set the current state to new state
        currentState = newState;

        // Start the current coroutine for the updated state
        StartCoroutine(currentState);
    }

    IEnumerator Idle()
    {
        while (currentState == "Idle")
        {
            // Perform idle behavior here
            if (targetTransform != null)
            {
                // If there is a target, go to the chasing state
                StartCoroutine(SwitchState("ChaseTarget"));
            }
            yield return null; // Wait for the next frame
        }
    }

    IEnumerator ChaseTarget()
    {
        // while loop in a coroutine = mini Update function
        while (currentState == "ChaseTarget")
        {
            // Perform chasing behavior here
            if (targetTransform == null)
            {
                StartCoroutine(SwitchState("Idle"));
            }
            else
            {
                myAgent.SetDestination(targetTransform.position);
            }
            
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // If the chaser 'sees' the player, set the target to the player
        if (other.gameObject.CompareTag("Player"))
            targetTransform = other.transform;
    }

    void OnTriggerExit(Collider other)
    {
        // If the player leaves the chaser's trigger, set the target to null
        if (other.gameObject.CompareTag("Player"))
            targetTransform = null;
    }
}