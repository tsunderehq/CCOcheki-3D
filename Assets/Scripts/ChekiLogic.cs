using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// for controlling the animations logic of the avatar, and also switching between the 2 avatars
/// </summary>
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
    [SerializeField] private Flash _flash;

    //for switching between Manaka and Kaguya
    [SerializeField] private GameObject ManakaAvatar, KaguyaAvatar;
    private GameObject CurrentAvatar;
    private Animator currentAnimator, manakaAnimator, kaguyaAnimator;


    //private bool countdownStarted = false; //prevent double countdown

    private const float smallHeartStopTime = 6f;
    private const float bigHeartStopTime = 5.8f;
    private const float nyanStopTime = 6.5f;

    private GameObject cdTimerInstance;
    public enum AnimationState
    {
        AnimatingBigHeart, AnimatingSmallHeart, AnimatingNyan, Finished
    }
    public AnimationState AnimState
    {
        get; set;
    }
    private void ChangeState(AnimationState newState)
    {
        if (AnimState != newState) AnimState = newState;
    }

    private void Start()
    {
        manakaAnimator = ManakaAvatar.GetComponent<Animator>();
        kaguyaAnimator = KaguyaAvatar.GetComponent<Animator>();

        SetManakaAvatar(); // set starting avatar as manaka
        ChangeState(AnimationState.Finished);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha0)) currentAnimator.Play("Waiting");
    //    if (Input.GetKeyDown(KeyCode.Alpha9)) currentAnimator.Play("Idle");

    //    // reset the Waiting start transform every finished loop
    //    if (currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Waiting"))
    //    {
    //        CurrentAvatar.transform.position = Hima.position;
    //        CurrentAvatar.transform.rotation = Hima.rotation;

    //        // randomize between the two hima animations
    //        if (Random.value > 0.5f) currentAnimator.SetBool("Hima", true);
    //        else currentAnimator.SetBool("Hima", false);
    //    }

    //    // wait for trigger cheki animation
    //    if (currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
    //    {
    //        //dont interrupt when in cheki animation
    //        if (currentAnimator.IsInTransition(0)) return;

    //        //press 1, 2, or 3 to trigger cheki animations
    //        if (Input.GetKeyDown(KeyCode.Alpha1)) DoBigHeartAnimation();
    //        if (Input.GetKeyDown(KeyCode.Alpha2)) DoSmallHeartAnimation();
    //        if (Input.GetKeyDown(KeyCode.Alpha3)) DoNyanAnimation();
    //    }

    //    //start countdown if 4 is pressed
    //    if (!countdownStarted && Input.GetKeyDown(KeyCode.Alpha4)) StartCountDown();
    //}

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

    public void PlayThroughAllAnimations()
    {
        StartCoroutine(PlayThroughAllAnimationsCoroutine());
    }

    public void GoNextAnimation()
    {
        AnimState += 1;
    }



    //private void SetWaiting(bool isWaiting)
    //{
    //    if (isWaiting)
    //    {
    //        CurrentAvatar.transform.position = Hima.position;
    //        CurrentAvatar.transform.rotation = Hima.rotation;
    //        currentAnimator.Play("Waiting");
    //    }
    //    else
    //    {
    //        CurrentAvatar.transform.position = Anim1.position;
    //        CurrentAvatar.transform.rotation = Anim1.rotation;
    //        currentAnimator.Play("Idle");
    //    }
    //}

    private void DoBigHeartAnimation()
    {
        CurrentAvatar.transform.position = Anim1.position;
        CurrentAvatar.transform.rotation = Anim1.rotation;
        currentAnimator.SetTrigger("BigHeart");
        StartCoroutine(PauseAnimatorCoroutine(bigHeartStopTime));
        //StartCountDown();
        ChangeState(AnimationState.AnimatingBigHeart);
    }

    private void DoSmallHeartAnimation()
    {
        CurrentAvatar.transform.position = Anim2.position;
        CurrentAvatar.transform.rotation = Anim2.rotation;
        currentAnimator.SetTrigger("SmallHeart");
        StartCoroutine(PauseAnimatorCoroutine(smallHeartStopTime));
        //StartCountDown();
        ChangeState(AnimationState.AnimatingSmallHeart);
    }

    private void DoNyanAnimation()
    {
        CurrentAvatar.transform.position = Anim3.position;
        CurrentAvatar.transform.rotation = Anim3.rotation;
        currentAnimator.SetTrigger("Nyan");
        StartCoroutine(PauseAnimatorCoroutine(nyanStopTime));
        //StartCountDown();
        ChangeState(AnimationState.AnimatingNyan);
    }

    private void StartCountDown()
    {
        //countdownStarted = true;
        cdTimerInstance = Instantiate(CountTimePrefab) as GameObject;
        cdTimerInstance.transform.SetParent(ParentCanvas.transform, false);
        cdTimerInstance.transform.localPosition = new Vector3(626f, 1286f, 0f);
    }

    public void CountDownEnd()
    {
        //countdownStarted = false;
        Texture2D screenshotData = screenshot.CaptureRenderTexture(mainCamera, 0);
        screenshotData.Apply();

        Destroy(cdTimerInstance);

        ScreenshotAnimator.texture = (Texture)screenshotData;
        screenShotAnimator.SetTrigger("Screenshot");

        if (!_flash.gameObject.activeInHierarchy) _flash.gameObject.SetActive(true);
        _flash.CameraFlash();

        currentAnimator.speed = 1; //we continue the animation
    }

    private IEnumerator PauseAnimatorCoroutine(float countdown)
    {
        yield return new WaitForSeconds(countdown);
        currentAnimator.speed = 0;
        StartCountDown();
    }

    private IEnumerator PlayThroughAllAnimationsCoroutine()
    {
        Debug.Log("doing big heart animation");
        DoBigHeartAnimation();
        while (AnimState == AnimationState.AnimatingBigHeart) yield return null;

        Debug.Log("doing small heart animation");
        DoSmallHeartAnimation();
        while (AnimState == AnimationState.AnimatingSmallHeart) yield return null;

        Debug.Log("doing nyan animation");
        DoNyanAnimation();
        while (AnimState == AnimationState.AnimatingNyan) yield return null;

        Debug.Log("finished all animation");
    }

}
