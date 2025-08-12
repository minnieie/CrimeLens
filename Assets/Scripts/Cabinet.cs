using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEditor.Rendering.Universal;

public class Cabinet : MonoBehaviour
{
    [Header("Door Transforms & Angles")]
    [SerializeField] Transform doorA;
    [SerializeField] Transform doorB;
    [SerializeField] Vector3 doorAOpenEuler = new Vector3(0f, -90f, 0f);
    [SerializeField] Vector3 doorBOpenEuler = new Vector3(0f, 90f, 0f);

    [Header("Kick Player Out")]
    [SerializeField] GameObject player;
    [SerializeField] float autoExitDelay = 7f; // Time before auto exit if player is hidden

    [Header("Timings (seconds)")]
    [SerializeField] float doorOpenDuration = 1f;
    [SerializeField] float doorCloseDuration = 1f;

    [Header("Exit Settings")]
    [SerializeField] Transform exitPoint;


    Vector3 doorAClosedEuler;
    Vector3 doorBClosedEuler;
    Vector3 doorAOpenEulerWorld;
    Vector3 doorBOpenEulerWorld;

    bool isAnimating = false;
    public bool isPlayerHidden = false;

    Vector3 storedPosition;
    Quaternion storedRotation;

    void Start()
    {
        doorAClosedEuler = doorA.localEulerAngles;
        doorBClosedEuler = doorB.localEulerAngles;
        doorAOpenEulerWorld = doorAClosedEuler + doorAOpenEuler;
        doorBOpenEulerWorld = doorBClosedEuler + doorBOpenEuler;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
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
        storedPosition = player.transform.position;
        storedRotation = player.transform.rotation;
        Debug.Log("player position before hiding: " + storedPosition);

        isPlayerHidden = true;

        // 3. Close doors
        doorA.localRotation = Quaternion.Euler(doorAClosedEuler);
        doorB.localRotation = Quaternion.Euler(doorBClosedEuler);
        yield return new WaitForSeconds(doorCloseDuration);

        isAnimating = false;
        StartCoroutine(AutoExitCabinetTimer());
    }

    IEnumerator ExitCabinetRoutine()
{
    isAnimating = true;

    // 1. Open doors
    doorA.localRotation = Quaternion.Euler(doorAOpenEulerWorld);
    doorB.localRotation = Quaternion.Euler(doorBOpenEulerWorld);
    yield return new WaitForSeconds(doorOpenDuration);

    // 2. Calculate exit position & rotation
    if (exitPoint != null)
    {
        player.transform.position = exitPoint.position;
        player.transform.rotation = exitPoint.rotation;
    }
    else
    {
        Debug.LogWarning("ExitPoint not assigned on Cabinet!");
    }


    isPlayerHidden = false;

    // 3. Close doors
    doorA.localRotation = Quaternion.Euler(doorAClosedEuler);
    doorB.localRotation = Quaternion.Euler(doorBClosedEuler);
    yield return new WaitForSeconds(doorCloseDuration);

    isAnimating = false;
}


    private IEnumerator AutoExitCabinetTimer()
    {
        yield return new WaitForSeconds(autoExitDelay);
        if (isPlayerHidden)
        {
            StartCoroutine(ExitCabinetRoutine());
        }
    }
}