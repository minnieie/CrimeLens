
 // This script controls a wandering AI character using Unity's NavMesh system.
 // The character alternates between idle and patrol states, moving between predefined patrol points.
 // After idling for a set duration, it navigates to the next patrol point.

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Wanderer : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points for the agent to visit
    public float idleTime = 2f; // Time to wait while idling before moving to next point

    private NavMeshAgent agent; // Reference to the NavMeshAgent component
    private int patrolIndex = 0; // Current index in the patrolPoints array

    // Enum to define the two states of the wanderer
    private enum WanderState { Idle, Patrol }
    private WanderState currentState = WanderState.Idle; // Initial state is Idle

    // Called when the script instance is being loaded
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
    }

    // Called before the first frame update
    void Start()
    {
        StartCoroutine(Idle()); // Start in Idle state
    }

    // Switches between Idle and Patrol states
    void SwitchState(WanderState newState)
    {
        if (currentState == newState) return; // Do nothing if already in desired state

        StopAllCoroutines(); // Stop any running coroutine

        currentState = newState; // Update the current state

        // Start the appropriate coroutine based on the new state
        if (newState == WanderState.Idle)
            StartCoroutine(Idle());
        else if (newState == WanderState.Patrol)
            StartCoroutine(Patrol());
    }

    // Coroutine for the Idle state
    IEnumerator Idle()
    {
        float timer = 0f;

        // Wait for idleTime seconds before switching to Patrol
        while (currentState == WanderState.Idle)
        {
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                SwitchState(WanderState.Patrol); // Switch to Patrol state
                yield break;
            }
            yield return null; // Wait for next frame
        }
    }

    // Coroutine for the Patrol state
    IEnumerator Patrol()
    {
        if (patrolPoints.Length == 0)
            yield break; // Exit if no patrol points are assigned

        // Set destination to the current patrol point
        agent.SetDestination(patrolPoints[patrolIndex].position);

        // Wait until the agent reaches the destination
        while (currentState == WanderState.Patrol)
        {
            // Check if the agent has reached the destination
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                // Move to the next patrol point (looping back to start if needed)
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                SwitchState(WanderState.Idle); // Switch back to Idle state
                yield break;
            }
            yield return null; // Wait for next frame
        }
    }
}
