using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// for controlling the animations logic of the avatar, and also switching between the 2 avatars
/// </summary>
public class ChekiLogic : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    [Header("Screenshot data")]
    //for screenshot function
    [SerializeField] private ScreenshotCompanion screenshot;
    [SerializeField] private RawImage ScreenshotAnimator;
    [SerializeField] private Animator screenShotAnimator;
    [SerializeField] private Flash _flash;

    [Header("UI elements")]
    //UI gameobjects
    public GameObject ParentCanvas, CountTimePrefab;
    [SerializeField] private TextMeshProUGUI avatarNameDisplay;
    [SerializeField] private TextMeshProUGUI animationNameDisplay;

    [Header("Texts")]
    [SerializeField] private string stanby_text = "";
    [SerializeField] private string bigHeart_text = "";
    [SerializeField] private string smallHeart_text = "";
    [SerializeField] private string neko_text = "";

    [Header("Animations")]
    //for returning manaka to original transform after animation
    public Transform Anim1, Anim2, Anim3, Hima;
    

    [Header("Avatar and animators")]
    //for switching between Manaka and Kaguya
    [SerializeField] private GameObject ManakaAvatar, KaguyaAvatar;
    private GameObject CurrentAvatar;
    private Animator currentAnimator, manakaAnimator, kaguyaAnimator;

    [Header("Sound effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip aclip_countdown;
    [SerializeField] private AudioClip aclip_intro;
    [SerializeField] private AudioClip aclip_outro;

    //private bool countdownStarted = false; //prevent double countdown

    private const float smallHeartStopTime = 6f;
    private const float bigHeartStopTime = 5.8f;
    private const float nyanStopTime = 6.5f;

    //new : Animations are longer so no need to stop, we check the end time and start the animation at end time - 3f;
    private const float bigHeartCountdownEndTime = 9.1f;
    private const float bigHeartTotalAnimationTime = 14.16f;

    private const float smallHeartCountdownEndTime = 9.1f;
    private const float smallHeartTotalAnimationTime = 16.7f;

    private const float nyanCountdownEndTime = 8.5f;
    private const float nyanTotalAnimationTime = 13.23f;

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
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        manakaAnimator = ManakaAvatar.GetComponent<Animator>();
        kaguyaAnimator = KaguyaAvatar.GetComponent<Animator>();
        animationNameDisplay.text = stanby_text;
        SetManakaAvatar(); // set starting avatar as manaka
        ChangeState(AnimationState.Finished);
    }

    public void InterruptAnimationAndReset()
    {
        StopAllCoroutines();
        ChangeState(AnimationState.Finished);
        currentAnimator.SetTrigger("Reset");
        audioSource.Stop();
        Debug.Log("interrupted");
    }

    public void SwitchAvatar()
    {
        if (KaguyaAvatar.activeInHierarchy) SetManakaAvatar();
        else SetKaguyaAvatar();
    }

    public void SetManakaAvatar()
    {
        KaguyaAvatar.SetActive(false);
        ManakaAvatar.SetActive(true);

        ManakaAvatar.transform.SetPositionAndRotation(Anim1.position, Anim1.rotation);

        CurrentAvatar = ManakaAvatar;
        currentAnimator = manakaAnimator;

        avatarNameDisplay.text = "Manaka";
    }

    public void SetKaguyaAvatar()
    {
        ManakaAvatar.SetActive(false);
        KaguyaAvatar.SetActive(true);

        KaguyaAvatar.transform.SetPositionAndRotation(Anim1.position, Anim1.rotation);

        CurrentAvatar = KaguyaAvatar;
        currentAnimator = kaguyaAnimator;

        avatarNameDisplay.text = "Kaguya";
    }

    public void PlayThroughAllAnimations()
    {
        StartCoroutine(PlayThroughAllAnimationsCoroutineAutomatic());
    }

    public void GoNextAnimation()
    {
        AnimState += 1;
    }


    private void DoBigHeartAnimation()
    {
        CurrentAvatar.transform.position = Anim1.position;
        CurrentAvatar.transform.rotation = Anim1.rotation;
        currentAnimator.SetTrigger("BigHeart");
        StartCoroutine(StartCountdownAfterDelay(bigHeartCountdownEndTime - 3f));
        //StartCountDown();
        ChangeState(AnimationState.AnimatingBigHeart);
    }

    private void DoSmallHeartAnimation()
    {
        CurrentAvatar.transform.position = Anim2.position;
        CurrentAvatar.transform.rotation = Anim2.rotation;
        currentAnimator.SetTrigger("SmallHeart");
        Debug.Log("triggered small heart");
        StartCoroutine(StartCountdownAfterDelay(smallHeartCountdownEndTime - 3f));
        //StartCountDown();
        ChangeState(AnimationState.AnimatingSmallHeart);
    }

    private void DoNyanAnimation()
    {
        CurrentAvatar.transform.position = Anim3.position;
        CurrentAvatar.transform.rotation = Anim3.rotation;
        currentAnimator.SetTrigger("Nyan");
        StartCoroutine(StartCountdownAfterDelay(nyanCountdownEndTime - 3f));
        //StartCountDown();
        ChangeState(AnimationState.AnimatingNyan);
    }

    private void StartCountDown()
    {
        //countdownStarted = true;
        cdTimerInstance = Instantiate(CountTimePrefab) as GameObject;
        cdTimerInstance.transform.SetParent(ParentCanvas.transform, false);
        cdTimerInstance.transform.localPosition = new Vector3(626f, 1286f, 0f);

        //play countdown sound
        audioSource.clip = aclip_countdown;
        audioSource.Play();
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
        ChangeState(AnimationState.Finished);
        ///currentAnimator.speed = 1; //we continue the animation
    }

    private IEnumerator PauseAnimatorCoroutine(float countdown)
    {
        yield return new WaitForSeconds(countdown);
        currentAnimator.speed = 0;
        StartCountDown();
    }

    private IEnumerator StartCountdownAfterDelay(float countdown)
    {
        yield return new WaitForSeconds(countdown);
        //currentAnimator.speed = 0;
        StartCountDown();
    }

    private IEnumerator PlayThroughAllAnimationsCoroutine()
    {
        //play intro music
        audioSource.clip = aclip_intro;
        audioSource.Play();
        yield return new WaitForSeconds(aclip_intro.length + 1); //wait until intro is over

        //play big heart animation
        Debug.Log("doing big heart animation");
        DoBigHeartAnimation();
        while (AnimState == AnimationState.AnimatingBigHeart) yield return null;

        yield return new WaitForSeconds(bigHeartTotalAnimationTime - bigHeartCountdownEndTime);
        //play small heart animation
        Debug.Log("doing small heart animation");
        DoSmallHeartAnimation();
        
        while (AnimState == AnimationState.AnimatingSmallHeart) yield return null;

        yield return new WaitForSeconds(smallHeartTotalAnimationTime - smallHeartCountdownEndTime);
        //play nyan animation
        Debug.Log("doing nyan animation");
        DoNyanAnimation();
        while (AnimState == AnimationState.AnimatingNyan) yield return null;

        Debug.Log("finished all animation");

        //outro sound
        audioSource.clip = aclip_outro;
        audioSource.Play();
    }


    //reworked to automatically cycle through all animations in animator with no transition with the idle animation
    private IEnumerator PlayThroughAllAnimationsCoroutineAutomatic()
    {
        //play intro music
        currentAnimator.SetTrigger("Start");
        audioSource.clip = aclip_intro;
        audioSource.Play();
        //yield return new WaitForSeconds(aclip_intro.length + 1); //wait until intro is over

        while (!currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("BigHeart")) yield return null;

        //play big heart animation
        animationNameDisplay.text = bigHeart_text;
        Debug.Log("doing big heart animation");
        StartCoroutine(StartCountdownAfterDelay(bigHeartCountdownEndTime - 3f));

        while (!currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("SmallHeart")) yield return null;

        //play small heart animation
        Debug.Log("doing small heart animation");
        animationNameDisplay.text = smallHeart_text;
        StartCoroutine(StartCountdownAfterDelay(smallHeartCountdownEndTime - 3f));
        while (!currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Nyan")) yield return null;

        //play nyan animation
        Debug.Log("doing nyan animation");

        animationNameDisplay.text = neko_text;
        StartCoroutine(StartCountdownAfterDelay(nyanCountdownEndTime - 3f));
        while (!currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Outro")) yield return null;

        Debug.Log("finished all animation");

        //outro sound
        audioSource.clip = aclip_outro;
        audioSource.Play();
        animationNameDisplay.text = stanby_text;
    }

}
