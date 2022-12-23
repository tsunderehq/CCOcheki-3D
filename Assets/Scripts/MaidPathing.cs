using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaidPathing : MonoBehaviour
{
    public Camera mainCamera;
    public Transform destinationPosition;
    public float moveSpeed;
    public float minDistanceToObj = 0.2f;

    public GameObject ParentCanvas, CountTimerGO;
    [SerializeField] ScreenshotCompanion screenshot;

    private Flash _flash;

    private enum Phase
    {
        WALK_UP, STAND, WALK_BACK, IDLE
    }
    private Phase currentPhase;
    Vector3 sourcePosition;
    Animator playerAnimator;

    private void Start()
    {
        sourcePosition = gameObject.transform.position;
        playerAnimator = GetComponent<Animator>();
        SetMovingAnimation();
        currentPhase = Phase.IDLE; // start with IDLE

        _flash = FindObjectOfType<Flash>();
    }

    private void Update()
    {
        // press Enter to start
        if (currentPhase == Phase.IDLE && Input.GetKeyDown(KeyCode.Return)) currentPhase = Phase.WALK_UP;

        switch (currentPhase)
        {
            case Phase.WALK_UP:
                WalkUpPhase();
                break;
            case Phase.STAND:
                StandingPhase();
                break;
            case Phase.WALK_BACK:
                WalkBackPhase();
                break;
        }
        
    }

    private void WalkUpPhase()
    {
        transform.position = Vector3.MoveTowards(transform.position, destinationPosition.position, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(destinationPosition.position, transform.position) < minDistanceToObj)
        {
            setStandingAnimation(); // we stop moving
            currentPhase = Phase.STAND;
        }
    }

    private void StandingPhase()
    {
        GameObject cdTimerInstance = Instantiate(CountTimerGO) as GameObject;
        cdTimerInstance.transform.SetParent(ParentCanvas.transform, false);

        //transform.LookAt(new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z));
    }

    private void WalkBackPhase()
    {
        SetMovingAnimation();
        transform.position = Vector3.MoveTowards(transform.position, sourcePosition, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(sourcePosition, transform.position) < minDistanceToObj)
        {
            currentPhase = Phase.IDLE;
        }
    }

    //triggers the "moving animation" in the animator. This is a placeholder and with the maid model it will be different
    private void SetMovingAnimation()
    {
        playerAnimator.SetBool("Walking", true);
        //playerAnimator.SetFloat("HorizontalMouvement", 0f);
        //playerAnimator.SetFloat("VerticalMouvement", 3f); //this is just to make the animator "walk"
    }

    private void setStandingAnimation()
    {
        playerAnimator.SetBool("Walking", false);
    }

    public void CountDownEnd()
    {
        screenshot.CaptureScreenshots(0, false);
        _flash.DoCameraFlash = true;
        currentPhase = Phase.WALK_BACK;
    }
}
