using System.Collections;
using UnityEngine;
using StarterAssets;

public class Cabinet : MonoBehaviour
{
    [Header("Door Transforms & Angles")]
    [SerializeField] Transform doorA;
    [SerializeField] Transform doorB;
    [SerializeField] Vector3 doorAOpenEuler = new Vector3(0f, -90f, 0f);
    [SerializeField] Vector3 doorBOpenEuler = new Vector3(0f, 90f, 0f);

    // [Header("Player & Hide Point")]
    // [SerializeField] GameObject player;
    // [SerializeField] Camera playerCamera;
    // [SerializeField] Transform hidePoint;
    // [SerializeField] Transform hideAnchor;

    [Header("Timings (seconds)")]
    [SerializeField] float doorOpenDuration = 1f;
    [SerializeField] float doorCloseDuration = 1f;

    Vector3 doorAClosedEuler;
    Vector3 doorBClosedEuler;
    Vector3 doorAOpenEulerWorld;
    Vector3 doorBOpenEulerWorld;

    bool isAnimating = false;
    bool isPlayerHidden = false;

    Vector3 storedPosition;
    Quaternion storedRotation;

    void Start()
    {
        doorAClosedEuler = doorA.localEulerAngles;
        doorBClosedEuler = doorB.localEulerAngles;
        doorAOpenEulerWorld = doorAClosedEuler + doorAOpenEuler;
        doorBOpenEulerWorld = doorBClosedEuler + doorBOpenEuler;
    }

    public void Interact()
    {
        if (isAnimating) return;

        if (!isPlayerHidden)
            StartCoroutine(EnterCabinetRoutine());
        else
            StartCoroutine(ExitCabinetRoutine());
    }

    IEnumerator EnterCabinetRoutine()
    {
        isAnimating = true;

        // 1. Open doors
        doorA.localRotation = Quaternion.Euler(doorAOpenEulerWorld);
        doorB.localRotation = Quaternion.Euler(doorBOpenEulerWorld);
        yield return new WaitForSeconds(doorOpenDuration);

        // 2. Store and move player
        // storedPosition = player.transform.position;
        // storedRotation = player.transform.rotation;

        // var controller = player.GetComponent<FirstPersonController>();
        // var cc = player.GetComponent<CharacterController>();
        // var rb = player.GetComponent<Rigidbody>();

        // if (controller != null) controller.enabled = false;
        // if (cc != null) cc.enabled = false;
        // if (rb != null)
        // {
        //     rb.isKinematic = true;
        // }

        // Debug.Log("Player position before hiding: " + storedPosition);
        // Debug.Log("Hide anchor position: " + hideAnchor.position);

        // if (rb != null)
        // {
        //     rb.MovePosition(hideAnchor.position);
        //     rb.MoveRotation(hidePoint.rotation * Quaternion.Euler(0f, -90f, 0f));
        // }
        // else
        // {
        //     player.transform.position = hideAnchor.position;
        //     player.transform.rotation = hidePoint.rotation * Quaternion.Euler(0f, -90f, 0f);
        // }

        // Debug.LogWarning("Player position after hiding: " + player.transform.position);

        isPlayerHidden = true;

        // 3. Close doors
        doorA.localRotation = Quaternion.Euler(doorAClosedEuler);
        doorB.localRotation = Quaternion.Euler(doorBClosedEuler);
        yield return new WaitForSeconds(doorCloseDuration);

        // 4. Re-enable movement
        // if (controller != null) controller.enabled = true;
        // if (cc != null) cc.enabled = true;
        // if (rb != null) rb.isKinematic = false;

        isAnimating = false;
    }

    IEnumerator ExitCabinetRoutine()
    {
        isAnimating = true;

        // 1. Open doors
        doorA.localRotation = Quaternion.Euler(doorAOpenEulerWorld);
        doorB.localRotation = Quaternion.Euler(doorBOpenEulerWorld);
        yield return new WaitForSeconds(doorOpenDuration);

        // 2. Move player to the Stored position
        // var controller = player.GetComponent<FirstPersonController>();
        // var cc = player.GetComponent<CharacterController>();
        // var rb = player.GetComponent<Rigidbody>();

        // if (controller != null) controller.enabled = false;
        // if (cc != null) cc.enabled = false;
        // if (rb != null)
        // {
        //     rb.isKinematic = true;
        // }

        // if (rb != null)
        // {
        //     rb.MovePosition(storedPosition);
        //     rb.MoveRotation(storedRotation);
        // }
        // else
        // {
        //     player.transform.position = storedPosition;
        //     player.transform.rotation = storedRotation;
        // }

        isPlayerHidden = false;

        // 3. Close doors
        doorA.localRotation = Quaternion.Euler(doorAClosedEuler);
        doorB.localRotation = Quaternion.Euler(doorBClosedEuler);
        yield return new WaitForSeconds(doorCloseDuration);

        // 4. Re-enable movement
        // if (controller != null) controller.enabled = true;
        // if (cc != null) cc.enabled = true;
        // if (rb != null) rb.isKinematic = false;

        isAnimating = false;
    }
}