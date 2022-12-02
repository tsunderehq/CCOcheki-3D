using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaidPathing : MonoBehaviour
{

    public Camera mainCamera;
    public Transform destinationPosition;
    public float moveSpeed;
    public float standingDuration;
    public float minDistanceToObj = 0.2f;

    private enum Phase
    {
        WALK_UP, STAND, WALK_BACK, OVER
    }
    private Phase currentPhase;
    private float remainingStandDuration;
    Vector3 sourcePosition;
    Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        sourcePosition = gameObject.transform.position;
        playerAnimator = GetComponent<Animator>();
        SetMovingAnimation();
        currentPhase = Phase.WALK_UP; // we start with walking up
        remainingStandDuration = standingDuration;
    }

    // Update is called once per frame
    void Update()
    {

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
            Debug.Log("Reached destination");
            setStandingAnimation(); // we stop moving
            currentPhase = Phase.STAND;
        }
    }

    private void StandingPhase()
    {
        transform.LookAt(new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z));
        remainingStandDuration -= Time.deltaTime;
        if (remainingStandDuration <= 0f)
        {
            currentPhase = Phase.WALK_BACK;
        }
    }

    private void WalkBackPhase()
    {
        SetMovingAnimation();
        transform.position = Vector3.MoveTowards(transform.position, sourcePosition, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(destinationPosition.position, transform.position) < minDistanceToObj)
        {
            Debug.Log("Reached destination");
            currentPhase = Phase.STAND;
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
}
