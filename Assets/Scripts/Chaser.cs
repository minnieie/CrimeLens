using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chaser : MonoBehaviour
{
    // Enum to keep track of AI states
    enum AIState { Idle, Patrol, Chase }

    // Patrol points assigned in the Inspector
    [SerializeField] private Transform[] patrolPoints;

    // How long AI waits in Idle state before patrolling
    [SerializeField] private float idleTime = 2f;

    // Reference to the player (set when detected)
    private Transform player;

    private NavMeshAgent agent;       // NavMeshAgent component for pathfinding
    private int patrolIndex = 0;      // Current patrol point index
    private AIState currentState = AIState.Idle;  // Start in Idle state
    private UnityEngine.Coroutine stateRoutine;   // Reference to currently running coroutine (state)

    // Called once when object is created
    void Awake()
    {
        // Get NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
    }

    // Called once before first frame update
    void Start()
    {
        // Start with Idle state coroutine
        stateRoutine = StartCoroutine(Idle());
    }

    // Called when another collider enters this object's trigger collider
    void OnTriggerEnter(Collider other)
    {
        // If player enters trigger, set player reference and start chasing
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            SwitchState(AIState.Chase);
        }
    }

    // Called when another collider leaves this object's trigger collider
    void OnTriggerExit(Collider other)
    {
        // If player leaves trigger, clear player reference and go back to Idle
        if (other.CompareTag("Player"))
        {
            player = null;
            SwitchState(AIState.Idle);
        }
    }

    // Method to change AI state safely
    void SwitchState(AIState newState)
    {
        // If already in this state, do nothing
        if (currentState == newState) return;

        // Stop the currently running state coroutine (if any)
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        // Update current state
        currentState = newState;

        // Start the coroutine corresponding to the new state
        if (newState == AIState.Idle)
            stateRoutine = StartCoroutine(Idle());
        else if (newState == AIState.Patrol)
            stateRoutine = StartCoroutine(Patrol());
        else if (newState == AIState.Chase)
            stateRoutine = StartCoroutine(Chase());
    }

    // Coroutine for Idle state behavior
    IEnumerator Idle()
    {
        float timer = 0f;

        // Keep looping while in Idle state
        while (currentState == AIState.Idle)
        {
            // Increase timer by elapsed time since last frame
            timer += Time.deltaTime;

            // If player detected, switch immediately to Chase state
            if (player != null)
            {
                SwitchState(AIState.Chase);
                yield break;  // Exit this coroutine
            }

            // If idle time has passed, switch to Patrol state
            if (timer >= idleTime)
            {
                SwitchState(AIState.Patrol);
                yield break;  // Exit this coroutine
            }

            yield return null;  // Wait until next frame
        }
    }

    // Coroutine for Patrol state behavior
    IEnumerator Patrol()
    {
        // If no patrol points assigned, do nothing
        if (patrolPoints.Length == 0)
            yield break;

        // Set agent's destination to current patrol point
        agent.SetDestination(patrolPoints[patrolIndex].position);

        // Keep looping while in Patrol state
        while (currentState == AIState.Patrol)
        {
            // If player detected, switch immediately to Chase state
            if (player != null)
            {
                SwitchState(AIState.Chase);
                yield break;
            }

            // Check if agent has reached the patrol point
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                // Move to the next patrol point index (loops back to 0 at end)
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;

                // After reaching a point, go to Idle state before next patrol
                SwitchState(AIState.Idle);
                yield break;
            }

            yield return null;  // Wait until next frame
        }
    }

    // Coroutine for Chase state behavior
    IEnumerator Chase()
    {
        // Keep looping while in Chase state
        while (currentState == AIState.Chase)
        {
            // If player lost (left trigger), switch to Idle state
            if (player == null)
            {
                SwitchState(AIState.Idle);
                yield break;
            }

            // Continuously set agent's destination to player's position
            agent.SetDestination(player.position);

            yield return null;  // Wait until next frame
        }
    }
}