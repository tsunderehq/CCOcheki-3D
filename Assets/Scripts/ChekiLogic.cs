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
        StartCountDown();
    }

    private void DoSmallHeartAnimation()
    {
        gameObject.transform.position = Anim2.position;
        gameObject.transform.rotation = Anim2.rotation;
        playerAnimator.SetTrigger("SmallHeart");
        StartCountDown();
    }

    private void DoNyanAnimation()
    {
        gameObject.transform.position = Anim3.position;
        gameObject.transform.rotation = Anim3.rotation;
        playerAnimator.SetTrigger("Nyan");
        StartCountDown();
    }

    private void StartCountDown()
    {
        GameObject cdTimerInstance = Instantiate(CountTimerGO) as GameObject;
        cdTimerInstance.transform.SetParent(ParentCanvas.transform, false);
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
