using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chaser : MonoBehaviour
{
    enum AIState { Idle, Patrol, Chase }

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private Transform player;

    private NavMeshAgent agent;
    private int patrolIndex = 0;
    private AIState currentState = AIState.Idle;
    private Coroutine stateRoutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        stateRoutine = StartCoroutine(HandleIdle());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            SwitchToState(AIState.Chase);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            SwitchToState(AIState.Idle);
        }
    }

    void SwitchToState(AIState newState)
    {
        if (currentState == newState) return;

        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        currentState = newState;

        switch (currentState)
        {
            case AIState.Idle:
                stateRoutine = StartCoroutine(HandleIdle());
                break;
            case AIState.Patrol:
                stateRoutine = StartCoroutine(HandlePatrol());
                break;
            case AIState.Chase:
                stateRoutine = StartCoroutine(HandleChase());
                break;
        }
    }

    IEnumerator HandleIdle()
    {
        float timer = 0f;

        while (currentState == AIState.Idle)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                SwitchToState(AIState.Chase);
                yield break;
            }

            if (timer >= idleTime)
            {
                SwitchToState(AIState.Patrol);
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator HandlePatrol()
    {
        if (patrolPoints.Length == 0)
            yield break;

        agent.SetDestination(patrolPoints[patrolIndex].position);

        while (currentState == AIState.Patrol)
        {
            if (player != null)
            {
                SwitchToState(AIState.Chase);
                yield break;
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                SwitchToState(AIState.Idle);
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator HandleChase()
    {
        while (currentState == AIState.Chase)
        {
            if (player == null)
            {
                SwitchToState(AIState.Idle);
                yield break;
            }

            agent.SetDestination(player.position);
            yield return null;
        }
    }
}
