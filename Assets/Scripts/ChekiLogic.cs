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
    public Transform Anim1, Anim2, Anim3, Hima1, Hima2;
    private Vector3 Anim1Pos, Anim2Pos, Anim3Pos, Hima1Pos, Hima2Pos;
    private Quaternion Anim1Rot, Anim2Rot, Anim3Rot, Hima1Rot, Hima2Rot;

    //camera flash effect
    private Flash _flash;

    //for controlling Manaka animator logic
    private Animator playerAnimator;
    

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        _flash = FindObjectOfType<Flash>();

        InitializeAnimationTransforms();
    }

    private void Update()
    {
        //only take input when in Idle mode
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || playerAnimator.IsInTransition(0)) return;

        //press 1, 2, or 3 to trigger cheki animations
        if (Input.GetKeyDown(KeyCode.Alpha1)) DoBigHeartAnimation();
        if (Input.GetKeyDown(KeyCode.Alpha2)) DoSmallHeartAnimation();
        if (Input.GetKeyDown(KeyCode.Alpha3)) DoNyanAnimation();

        //press 9, 0 to trigger standby animations
        if (Input.GetKeyDown(KeyCode.Alpha9)) DoHima1Animation();
        if (Input.GetKeyDown(KeyCode.Alpha0)) DoHima2Animation();
    }

    private void DoBigHeartAnimation()
    {
        gameObject.transform.position = Anim1Pos;
        gameObject.transform.rotation = Anim1Rot;
        playerAnimator.SetTrigger("BigHeart");
        StartCountDown();
    }

    private void DoSmallHeartAnimation()
    {
        gameObject.transform.position = Anim2Pos;
        gameObject.transform.rotation = Anim2Rot;
        playerAnimator.SetTrigger("SmallHeart");
        StartCountDown();
    }

    private void DoNyanAnimation()
    {
        gameObject.transform.position = Anim3Pos;
        gameObject.transform.rotation = Anim3Rot;
        playerAnimator.SetTrigger("Nyan");
        StartCountDown();
    }

    private void DoHima1Animation()
    {
        gameObject.transform.position = Hima1Pos;
        gameObject.transform.rotation = Hima1Rot;
        playerAnimator.SetTrigger("Hima1");
    }

    private void DoHima2Animation()
    {
        gameObject.transform.position = Hima2Pos;
        gameObject.transform.rotation = Hima2Rot;
        playerAnimator.SetTrigger("Hima2");
    }

    private void StartCountDown()
    {
        GameObject cdTimerInstance = Instantiate(CountTimerGO) as GameObject;
        cdTimerInstance.transform.SetParent(ParentCanvas.transform, false);
    }

    private void InitializeAnimationTransforms()
    {
        Anim1Pos = Anim1.position;
        Anim1Rot = Anim1.rotation;

        Anim2Pos = Anim2.position;
        Anim2Rot = Anim2.rotation;

        Anim3Pos = Anim3.position;
        Anim3Rot = Anim3.rotation;

        Hima1Pos = Hima1.position;
        Hima1Rot = Hima1.rotation;

        Hima2Pos = Hima2.position;
        Hima2Rot = Hima2.rotation;
    }

    public void CountDownEnd()
    {
        Texture2D screenshotData = screenshot.CaptureRenderTexture(mainCamera, 0);
        screenshotData.Apply();
        ScreenshotAnimator.texture = (Texture)screenshotData;
        
        screenShotAnimator.SetTrigger("Screenshot");
        _flash.DoCameraFlash = true;
    }
}
