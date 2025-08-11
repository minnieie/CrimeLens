using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerBehaviour : MonoBehaviour
{
    // Interaction variables
    bool canInteract;
    CoinBehaviour nearbyCoin;
    DoorBehaviour currentDoor;
    ComputerBehaviour computer;
    USBBehaviour usb; 
    private Cabinet currentCabinet;
    NPCBehaviour npc;
    private CoinBehaviour lastCoin = null;

    // Movement variables
    public AudioSource footstepAudio;
    public float moveThreshold = 0.1f;
    private Vector3 lastPosition;
    private DeskDrawer currentDeskDrawer;

    // Configuration
    [SerializeField] Transform spawnPoint;
    [SerializeField] float interactionDistance = 3.0f;

    // Store the last frame's interaction state
    private bool hadInteractableLastFrame = false;

    // Respawn point for the player
    public Vector3 respawnPoint; // Assign this in the Inspector

    // Captcha UI
    public bool isUILocked = false;

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
        currentDeskDrawer = null;


        // Perform raycast detection
        if (Camera.main != null)
        {
            Vector3 rayOrigin = Camera.main.transform.position;
            Vector3 rayDirection = Camera.main.transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, interactionDistance))
            {   
                Debug.DrawRay(rayOrigin, rayDirection * hitInfo.distance, Color.green);
                HandleInteractionTarget(hitInfo.collider.gameObject);
            }
            else
            {
                ClearLastCoinHighlight();
                
                // Clear computer reference if no computer hit
                if (computer != null)
                {
                    computer = null;
                    canInteract = false;
                }
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
        Debug.Log("Raycast hit: " + target.name);

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
        else if (target.CompareTag("DeskDrawer"))
        {
            canInteract = true;
            currentDeskDrawer = target.GetComponent<DeskDrawer>();
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
                // Debug.Log($"Computer found - canInteract set to true");
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
        else if (target.CompareTag("USB"))
        {
            if (usb == null)  // only assign if not holding USB already
            {
                usb = target.GetComponent<USBBehaviour>();
                if (usb != null)
                {
                    canInteract = true;
                    Debug.Log("USB found - canInteract set to true");
                }
            }
        }
        else
        {
            if (usb != null && !usb.isPickedUp)
            {
                usb = null;
            }
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
        Debug.Log($"OnInteract called. ActiveComputer: {ComputerBehaviour.ActiveComputer?.gameObject.name}, CurrentComputer: {computer?.gameObject.name}");

        // If currently interacting with a computer...
        if (ComputerBehaviour.ActiveComputer != null)
        {
            // Only end interaction if the player is still aiming at the same computer
            if (computer == ComputerBehaviour.ActiveComputer)
            {
                Debug.Log($"Quitting computer: {ComputerBehaviour.ActiveComputer.gameObject.name}");
                ComputerBehaviour.ActiveComputer.EndInteraction();
                computer = null; // Clear reference after quitting
                return; // Return early so we don't start interaction again immediately
            }
            else
            {
                // Player is interacting with one computer, but aiming at another
                Debug.Log("Player is interacting with a different computer; ignoring interact.");
                return;
            }
        }

        Debug.Log("USB reference: " + usb);
        Debug.Log("USB picked up state: " + (usb != null ? usb.isPickedUp.ToString() : "null"));
        
        if (usb != null && !usb.isPickedUp)
        {
            Debug.Log("Attempting to pick up USB...");
            usb.PickUpUSB();
            return;
        }


        if (usb != null && usb.isPickedUp && computer != null && computer.isUSBOnly)
        {
            usb.playerCameraTransform = Camera.main.transform;  // <-- Assign camera here
            bool inserted = usb.TryInsertIntoComputer();
            if (inserted)
            {
                Debug.Log("USB inserted into USB-only computer.");
            }
            else
            {
                Debug.Log("Failed to insert USB (maybe too far?).");
            }
            return;
        }


        // If not interacting and player is aiming at a computer
        if (computer != null)
        {
            if (computer.isUSBOnly)
            {
                // USB insertion handled below, so just return here to skip normal interaction UI
                return;
            }
            else
            {
                if (!computer.IsInteracting)
                {
                    Debug.Log($"Interacting with computer: {computer.gameObject.name}");
                    computer.StartInteraction();
                    return;
                }
            }
        }

        // Other interactions
        if (nearbyCoin != null)
        {
            nearbyCoin.Collect(this);
            return;
        }
        else if (currentDoor != null)
        {
            currentDoor.OpenDoors();
            return;
        }
        else if (npc != null)
        {
            Debug.Log("Dialogue interaction triggered.");
            NPCBehaviour.dialogueActive = true;
            npc.StartDialogue();
            return;
        }
        else if (currentCabinet != null)
        {
            Debug.Log("Interacting with Cabinet: " + currentCabinet.gameObject.name);
            currentCabinet.Interact();
            return;
        }
        else if (currentDeskDrawer != null)
        {
            currentDeskDrawer.Interact();
            return;
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