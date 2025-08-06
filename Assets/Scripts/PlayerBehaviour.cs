using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Interaction variables
    bool canInteract;
    CoinBehaviour nearbyCoin;
    DoorBehaviour currentDoor;
    ComputerBehaviour computer;
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

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Store previous state before resetting
        hadInteractableLastFrame = canInteract;

        // Reset interaction state (but preserve computer reference)
        canInteract = false;
        nearbyCoin = null;
        currentDoor = null;
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
            Debug.Log($"Interacting with coin: {nearbyCoin.gameObject.name}");
            nearbyCoin.Collect(this);
        }
        else if (currentDoor != null)
        {
            Debug.Log($"Interacting with door: {currentDoor.gameObject.name}");
            currentDoor.OpenDoors();
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
        else if (npc != null)
        {
            Debug.Log("Dialogue interaction triggered.");
            NPCBehaviour.dialogueActive = true;
            npc.StartDialogue();
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
}