using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChekiLogic : MonoBehaviour
{
    public Camera mainCamera;

    //for screenshot function
    [SerializeField] private ScreenshotCompanion screenshot;
    [SerializeField] private RawImage ScreenshotAnimator;
    [SerializeField] private Animator screenShotAnimator;

    //UI gameobjects
    public GameObject ParentCanvas, CountTimePrefab;

    //for returning manaka to original transform after animation
    public Transform Anim1, Anim2, Anim3, Hima;

    //camera flash effect
    private Flash _flash;

    //for switching between Manaka and Kaguya
    [SerializeField] private GameObject ManakaAvatar, KaguyaAvatar;
    private GameObject CurrentAvatar;
    private Animator currentAnimator, manakaAnimator, kaguyaAnimator;


    private bool countdownStarted = false; //prevent double countdown

    private const float smallHeartStopTime = 6f;
    private const float bigHeartStopTime = 5.8f;
    private const float nyanStopTime = 6.5f;

    private void Start()
    {
        manakaAnimator = ManakaAvatar.GetComponent<Animator>();
        kaguyaAnimator = KaguyaAvatar.GetComponent<Animator>();

        SetKaguyaAvatar();

        _flash = FindObjectOfType<Flash>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) currentAnimator.Play("Waiting");
        if (Input.GetKeyDown(KeyCode.Alpha9)) currentAnimator.Play("Idle");

        // reset the Waiting start transform every finished loop
        if (currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Waiting"))
        {
            CurrentAvatar.transform.position = Hima.position;
            CurrentAvatar.transform.rotation = Hima.rotation;

            // randomize between the two hima animations
            if (Random.value > 0.5f) currentAnimator.SetBool("Hima", true);
            else currentAnimator.SetBool("Hima", false);
        }

        // wait for trigger cheki animation
        if (currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //dont interrupt when in cheki animation
            if (currentAnimator.IsInTransition(0)) return;

            //press 1, 2, or 3 to trigger cheki animations
            if (Input.GetKeyDown(KeyCode.Alpha1)) DoBigHeartAnimation();
            if (Input.GetKeyDown(KeyCode.Alpha2)) DoSmallHeartAnimation();
            if (Input.GetKeyDown(KeyCode.Alpha3)) DoNyanAnimation();
        }

        //start countdown if 4 is pressed
        if (!countdownStarted && Input.GetKeyDown(KeyCode.Alpha4)) StartCountDown();
    }

    public void SetManakaAvatar()
    {
        KaguyaAvatar.SetActive(false);
        ManakaAvatar.SetActive(true);

        CurrentAvatar = ManakaAvatar;
        currentAnimator = manakaAnimator;
    }

    public void SetKaguyaAvatar()
    {
        ManakaAvatar.SetActive(false);
        KaguyaAvatar.SetActive(true);

        CurrentAvatar = KaguyaAvatar;
        currentAnimator = kaguyaAnimator;
    }


    private void SetWaiting(bool isWaiting)
    {
        if (isWaiting)
        {
            CurrentAvatar.transform.position = Hima.position;
            CurrentAvatar.transform.rotation = Hima.rotation;
            currentAnimator.Play("Waiting");
        }
        else
        {
            CurrentAvatar.transform.position = Anim1.position;
            CurrentAvatar.transform.rotation = Anim1.rotation;
            currentAnimator.Play("Idle");
        }
    }

    private void DoBigHeartAnimation()
    {
        CurrentAvatar.transform.position = Anim1.position;
        CurrentAvatar.transform.rotation = Anim1.rotation;
        currentAnimator.SetTrigger("BigHeart");
        StartCoroutine(PauseAnimatorCoroutine(bigHeartStopTime));
        //StartCountDown();
    }

    private void DoSmallHeartAnimation()
    {
        CurrentAvatar.transform.position = Anim2.position;
        CurrentAvatar.transform.rotation = Anim2.rotation;
        currentAnimator.SetTrigger("SmallHeart");
        StartCoroutine(PauseAnimatorCoroutine(smallHeartStopTime));
        //StartCountDown();
    }

    private void DoNyanAnimation()
    {
        CurrentAvatar.transform.position = Anim3.position;
        CurrentAvatar.transform.rotation = Anim3.rotation;
        currentAnimator.SetTrigger("Nyan");
        StartCoroutine(PauseAnimatorCoroutine(nyanStopTime));
        //StartCountDown();
    }

    private void StartCountDown()
    {
        countdownStarted = true;
        GameObject cdTimerInstance = Instantiate(CountTimePrefab) as GameObject;
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
        currentAnimator.speed = 1; //we continue the animation
    }

    IEnumerator PauseAnimatorCoroutine(float countdown)
    {
        yield return new WaitForSeconds(countdown);
        currentAnimator.speed = 0;
    }
}
