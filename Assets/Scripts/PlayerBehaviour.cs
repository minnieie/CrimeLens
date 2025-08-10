using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerBehaviour : MonoBehaviour
{
    // Interaction variables
    bool canInteract;
    CoinBehaviour nearbyCoin;
    DoorBehaviour currentDoor;
    ComputerBehaviour computer;
    private Cabinet currentCabinet;
    NPCBehaviour npc;
    private CoinBehaviour lastCoin = null;

    // Movement variables
    public AudioSource footstepAudio;
    public float moveThreshold = 0.1f;
    private Vector3 lastPosition;

    // Configuration
    [SerializeField] Transform spawnPoint;
    [SerializeField] float interactionDistance = 3.0f;

    // Store the last frame's interaction state
    private bool hadInteractableLastFrame = false;

    // Respawn point for the player
    public Vector3 respawnPoint; // Assign this in the Inspector

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (GameManager.instance.useReturnSpawn && currentScene == "lobby")
        {
            transform.position = GameManager.instance.returnSpawnPosition;
            GameManager.instance.useReturnSpawn = false;
            Debug.Log("Spawned at saved lobby position: " + transform.position);
        }
        else
        {
            Debug.Log("Using default spawn in: " + currentScene);
        }
    }


    void Update()
    {
        // Store previous state before resetting
        hadInteractableLastFrame = canInteract;

        // Reset interaction state (but preserve computer reference)
        canInteract = false;
        nearbyCoin = null;
        currentDoor = null;
        currentCabinet = null;
        npc = null;

        // Perform raycast detection
        if (Camera.main != null)
        {
            Vector3 rayOrigin = Camera.main.transform.position;
            Vector3 rayDirection = Camera.main.transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, interactionDistance))
            {
                HandleInteractionTarget(hitInfo.collider.gameObject);
            }
            else
            {
                ClearLastCoinHighlight();
            }
        }

        // Get raw input values (not affected by smoothing)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Play footsteps if moving (walking or sprinting)
        if (isMoving)
        {
            if (!footstepAudio.isPlaying)
            {
                // Adjust pitch/volume based on sprinting
                footstepAudio.pitch = isSprinting ? 1.2f : 1f;
                footstepAudio.volume = isSprinting ? 0.8f : 0.6f;
                footstepAudio.Play();
            }
        }
        else
        {
            if (footstepAudio.isPlaying)
            {
                footstepAudio.Stop();
            }
        }
    }

    void HandleInteractionTarget(GameObject target)
    {
        if (target.CompareTag("Collectible"))
        {
            canInteract = true;
            nearbyCoin = target.GetComponent<CoinBehaviour>();
            if (nearbyCoin != null)
            {
                nearbyCoin.Highlight();
                lastCoin = nearbyCoin;
            }
        }
        else if (target.CompareTag("Door"))
        {
            canInteract = true;
            currentDoor = target.GetComponent<DoorBehaviour>();
        }
        else if (target.CompareTag("Computer"))
        {
            computer = target.GetComponent<ComputerBehaviour>() ?? target.GetComponentInParent<ComputerBehaviour>();
            if (computer != null)
            {
                canInteract = true;
                Debug.Log($"Computer found - canInteract set to true");
            }
        }
        else if (target.CompareTag("NPC"))
        {
            npc = target.GetComponent<NPCBehaviour>();
            if (npc != null)
            {
                canInteract = true;
                Debug.Log("NPC found - canInteract set to true");
            }
        }
        else if (target.CompareTag("Cabinet"))
        {   
            canInteract = true;
            currentCabinet = target.GetComponent<Cabinet>();
            // Debug.Log("Cabinet found - canInteract set to true");
        }
        else
        {
            ClearLastCoinHighlight();
        }
    }

    void ClearLastCoinHighlight()
    {
        if (lastCoin != null)
        {
            lastCoin.Unhighlight();
            lastCoin = null;
        }
    }

    public void OnInteract()
    {
        if (nearbyCoin != null)
        {
            nearbyCoin.Collect(this);
        }
        else if (currentDoor != null)
        {
            currentDoor.OpenDoors();
        }
        else if (npc != null)
        {
            Debug.Log("Dialogue interaction triggered.");
            NPCBehaviour.dialogueActive = true;
            npc.StartDialogue();
        }
        else if (currentCabinet != null)  
        {   
            Debug.Log("Interacting with Cabinet: " + currentCabinet.gameObject.name);
            currentCabinet.Interact();
        }
        else if (ComputerBehaviour.ActiveComputer != null)
        {
            Debug.Log($"Quitting computer: {ComputerBehaviour.ActiveComputer.gameObject.name}");
            ComputerBehaviour.ActiveComputer.EndInteraction();
        }
        else if (computer != null)
        {
            if (!computer.IsInteracting)
            {
                Debug.Log($"Interacting with computer: {computer.gameObject.name}");
                computer.StartInteraction();
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        LevelChanger levelChanger = other.GetComponent<LevelChanger>();
        if (levelChanger != null)
        {
            return;
        }
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("Respawn point set to: " + respawnPoint);
    }

}