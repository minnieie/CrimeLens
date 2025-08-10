using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chaser : MonoBehaviour
{
    // Enum to keep track of AI states
    enum AIState { Idle, Patrol, Chase, Investigate }

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Patrol points assigned in the Inspector
    [SerializeField] private float idleTime = 2f;      // How long AI waits in Idle state before patrolling

    [Header("Alert Settings")]
    [SerializeField] private int maxStrikes = 3;                      // Max number of detection strikes
    [SerializeField] private float strikeCooldown = 30f;             // Time to reduce strike after inactivity
    [SerializeField] private float hearingRadius = 5f;               // Radius to detect sounds
    [SerializeField] private float hearingAngle = 90f;               // Angle to detect sounds
    [SerializeField] private float strikeCooldownDuration = 2f;      // Cooldown before a new strike can be triggered

    private NavMeshAgent agent;        // Used to move the NPC around using Unity's pathfinding
    private Transform player;          // Reference to player
    private Vector3 lastHeardPosition; // Last position where sound was detected
    private float investigateWaitTime = 3f; // Duration NPC waits while investigating

    private int patrolIndex = 0;       // Current patrol point index
    private int currentStrikes = 0;    // How many times the player has been detected
    private float currentCooldown = 0f;      // Timer to reduce strike count
    private float strikeCooldownTimer = 0f;  // Prevents spamming strikes too quickly

    private AIState currentState = AIState.Idle;         // Current AI state
    private UnityEngine.Coroutine stateRoutine;         // Keeps track of which coroutine is running

    // Event triggered on third strike
    public static event System.Action OnThirdStrike;

    [Header("CallForBackup Settings")]
    [SerializeField] private GameObject[] npcPrefabs; // Prefab to instantiate for backup
    [SerializeField] float spawnRadiusMin = 1f;
    [SerializeField] float spawnRadiusMax = 2f;
    [SerializeField] bool hasSpawned = false; 
    [SerializeField] float backupCooldown = 5f; // Cooldown for calling backup
    [SerializeField] float spawnGracePeriod = 3f; // How long backup NPCs stay
    float lastBackupTime = -Mathf.Infinity;

    // Runs when the game starts (before Start)
    void Awake()
    {
        // Get NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
    }

    // Called before the first frame update
    void Start()
    {
        // Start with Idle state coroutine
        stateRoutine = StartCoroutine(Idle());
    }

    // Called once per frame
    void Update()
    {
        // Reduce strike cooldown over time
        if (strikeCooldownTimer > 0f)
            strikeCooldownTimer -= Time.deltaTime;

        // Hearing-based detection (only when not chasing)
        if (currentState != AIState.Chase && strikeCooldownTimer <= 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, hearingRadius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Vector3 directionToPlayer = hit.transform.position - transform.position;
                    float angle = Vector3.Angle(transform.forward, directionToPlayer);

                    if (angle <= hearingAngle / 2f)
                    {
                        PlayerBehaviour pb = hit.GetComponent<PlayerBehaviour>();
                        if (pb != null && pb.footstepAudio.isPlaying)
                        {
                            player = hit.transform;
                            lastHeardPosition = player.position;

                            Debug.Log($"NPC heard player! Distance: {directionToPlayer.magnitude:F2}, Angle: {angle:F2}");

                            RegisterStrike();                         // Register a detection
                            SwitchState(AIState.Investigate);         // Switch to investigate state
                            strikeCooldownTimer = strikeCooldownDuration; // Reset strike timer
                            break;
                        }
                    }
                }
            }
        }

        // Strike reduction logic
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
                ReduceStrike();
        }
    }

    // Called when another collider enters this object's trigger collider
    void OnTriggerEnter(Collider other)
    {
        // If player enters trigger, set player reference and start chasing
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            RegisterStrike();
            SwitchState(AIState.Chase);
        }
    }

    // Called when another collider leaves this object's trigger collider
    void OnTriggerExit(Collider other)
    {
        // If player leaves trigger, clear player reference and go back to Idle
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left trigger zone.");
            player = null;
            SwitchState(AIState.Idle);
        }
    }

    // Method to change AI state safely
    void SwitchState(AIState newState)
    {
        // If already in desired state, do nothing
        if (currentState == newState) return;

        // Stop currently running coroutine
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        // Update to new state and start respective coroutine
        currentState = newState;

        if (newState == AIState.Idle)
            stateRoutine = StartCoroutine(Idle());
        else if (newState == AIState.Patrol)
            stateRoutine = StartCoroutine(Patrol());
        else if (newState == AIState.Chase)
            stateRoutine = StartCoroutine(Chase());
        else if (newState == AIState.Investigate)
            stateRoutine = StartCoroutine(Investigate());
    }

    // Coroutine for Idle state behavior
    IEnumerator Idle()
    {
        float timer = 0f;

        while (currentState == AIState.Idle)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                SwitchState(AIState.Chase);
                yield break;
            }

            if (timer >= idleTime)
            {
                SwitchState(AIState.Patrol);
                yield break;
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
                Debug.Log("Player lost. Returning to Idle.");
                SwitchState(AIState.Idle);
                yield break;
            }

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > hearingRadius + 5f)
            {
                Debug.Log($"Player too far ({dist:F2}). Stopping chase.");
                player = null;
                SwitchState(AIState.Idle);
                yield break;
            }

            // Continuously set agent's destination to player's position
            agent.SetDestination(player.position);
            yield return null;
        }
    }

    // Coroutine for Investigate state behavior
    IEnumerator Investigate()
    {
        Debug.Log("Investigating last heard position: " + lastHeardPosition);
        agent.SetDestination(lastHeardPosition);

        // Wait until reaching investigation point
        while (currentState == AIState.Investigate && agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        if (currentState != AIState.Investigate)
            yield break;

        Debug.Log("Reached investigation point. Waiting...");

        float timer = 0f;
        while (timer < investigateWaitTime)
        {
            transform.Rotate(0, 120f * Time.deltaTime, 0); // Look around while waiting

            if (player != null)
            {
                Debug.Log("Player detected during investigation. Switching to Chase.");
                SwitchState(AIState.Chase);
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Investigation complete. Returning to Patrol.");
        SwitchState(AIState.Patrol);
    }

    IEnumerator CallForBackup()
    {
        if (Time.time - lastBackupTime < backupCooldown)
        {
            SwitchState(AIState.Idle);
            yield break;
        }

        lastBackupTime = Time.time;

        hasSpawned = true;
        for (int i = 0; i < 3; i++)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere;
            randomOffset.y = 0; // Keep spawn position on the ground
            randomOffset = randomOffset.normalized * UnityEngine.Random.Range(spawnRadiusMin, spawnRadiusMax);
            Vector3 spawnPosition = transform.position + randomOffset;

            int prefabIndex = Random.Range(0, npcPrefabs.Length);
            GameObject chosenPrefab = npcPrefabs[prefabIndex];
            
            Instantiate(chosenPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Backup NPC spawned at: " + spawnPosition);
        }

    }

    // Handle detection logic
    private void RegisterStrike()
    {
        if (currentStrikes >= maxStrikes) return;

        currentCooldown = strikeCooldown;
        currentStrikes++;
        Debug.Log($"Strike {currentStrikes}/{maxStrikes} registered.");

        if (currentStrikes >= maxStrikes)
        {
            Debug.Log("Max strikes reached. Calling for backup!");
            OnThirdStrike?.Invoke();
            StartCoroutine(CallForBackup());
        }
    }

    // Reduce strikes after cooldown period
    private void ReduceStrike()
    {
        if (currentStrikes > 0)
        {
            currentStrikes--;
            currentCooldown = strikeCooldown;
            Debug.Log($"Strike reduced: {currentStrikes}/{maxStrikes} remaining.");
        }
    }
}
