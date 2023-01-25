using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChekiLogic : MonoBehaviour
{
    public Camera mainCamera;

    //for screenshot function
    [SerializeField] ScreenshotCompanion screenshot;
    public RawImage ScreenshotAnimator;
    public Animator screenShotAnimator;

    //UI gameobjects
    public GameObject ParentCanvas, CountTimerGO;

    //for returning manaka to original transform after animation
    public Transform Anim1, Anim2, Anim3, Hima;

    //camera flash effect
    private Flash _flash;

    //for controlling Manaka animator logic
    private Animator playerAnimator;

    private bool countdownStarted = false; //prevent double countdown

    private const float smallHeartStopTime = 6f;
    private const float bigHeartStopTime = 5.8f;
    private const float nyanStopTime = 6.5f;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        _flash = FindObjectOfType<Flash>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) playerAnimator.Play("Waiting");
        if (Input.GetKeyDown(KeyCode.Alpha9)) playerAnimator.Play("Idle");

        // reset the Waiting start transform every finished loop
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Waiting"))
        {
            gameObject.transform.position = Hima.position;
            gameObject.transform.rotation = Hima.rotation;

            // randomize between the two hima animations
            if (Random.value > 0.5f) playerAnimator.SetBool("Hima", true);
            else playerAnimator.SetBool("Hima", false);
        }

        // wait for trigger cheki animation
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //dont interrupt when in cheki animation
            if (playerAnimator.IsInTransition(0)) return;

            //press 1, 2, or 3 to trigger cheki animations
            if (Input.GetKeyDown(KeyCode.Alpha1)) DoBigHeartAnimation();
            if (Input.GetKeyDown(KeyCode.Alpha2)) DoSmallHeartAnimation();
            if (Input.GetKeyDown(KeyCode.Alpha3)) DoNyanAnimation();
        }

        //start countdown if 4 is pressed
        if (!countdownStarted && Input.GetKeyDown(KeyCode.Alpha4)) StartCountDown();
    }

    private void SetWaiting(bool isWaiting)
    {
        if (isWaiting)
        {
            gameObject.transform.position = Hima.position;
            gameObject.transform.rotation = Hima.rotation;
            playerAnimator.Play("Waiting");
        }
        else
        {
            gameObject.transform.position = Anim1.position;
            gameObject.transform.rotation = Anim1.rotation;
            playerAnimator.Play("Idle");
        }
    }

    private void DoBigHeartAnimation()
    {
        gameObject.transform.position = Anim1.position;
        gameObject.transform.rotation = Anim1.rotation;
        playerAnimator.SetTrigger("BigHeart");
        StartCoroutine(PauseAnimatorCoroutine(bigHeartStopTime));
        //StartCountDown();
    }

    private void DoSmallHeartAnimation()
    {
        gameObject.transform.position = Anim2.position;
        gameObject.transform.rotation = Anim2.rotation;
        playerAnimator.SetTrigger("SmallHeart");
        StartCoroutine(PauseAnimatorCoroutine(smallHeartStopTime));
        //StartCountDown();
    }

    private void DoNyanAnimation()
    {
        gameObject.transform.position = Anim3.position;
        gameObject.transform.rotation = Anim3.rotation;
        playerAnimator.SetTrigger("Nyan");
        StartCoroutine(PauseAnimatorCoroutine(nyanStopTime));
        //StartCountDown();
    }

    private void StartCountDown()
    {
        countdownStarted = true;
        GameObject cdTimerInstance = Instantiate(CountTimerGO) as GameObject;
        cdTimerInstance.transform.SetParent(ParentCanvas.transform, false);
    }

    public void CountDownEnd()
    {
        countdownStarted = false;
        Texture2D screenshotData = screenshot.CaptureRenderTexture(mainCamera, 0);
        screenshotData.Apply();
        ScreenshotAnimator.texture = (Texture)screenshotData;
        
        screenShotAnimator.SetTrigger("Screenshot");
        _flash.DoCameraFlash = true;
        playerAnimator.speed = 1; //we continue the animation
    }

    IEnumerator PauseAnimatorCoroutine(float countdown)
    {
        yield return new WaitForSeconds(countdown);
        playerAnimator.speed = 0;
    }
}
