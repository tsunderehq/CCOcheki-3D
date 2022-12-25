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
    public Transform ManakaTransform;
    private Vector3 ManakaStartPos;
    private Quaternion ManakaStartRot;

    //camera flash effect
    private Flash _flash;

    //for controlling Manaka animator logic
    private Animator playerAnimator;
    

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        _flash = FindObjectOfType<Flash>();

        ManakaStartPos = ManakaTransform.position;
        ManakaStartRot = ManakaTransform.rotation;
    }

    private void Update()
    {
        //press 1, 2, or 3 to trigger animation. Does not interrupt until back to Idle state
        if (Input.GetKeyDown(KeyCode.Alpha1)) DoBigHeartAnimation();
        if (Input.GetKeyDown(KeyCode.Alpha2)) DoSmallHeartAnimation();
        if (Input.GetKeyDown(KeyCode.Alpha3)) DoNyanAnimation();

        //return Manaka to initial transform after doing cheki animation
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !playerAnimator.IsInTransition(0))
        {
            if (gameObject.transform.position != ManakaStartPos && gameObject.transform.rotation != ManakaStartRot)
            {
                gameObject.transform.position = ManakaStartPos;
                gameObject.transform.rotation = ManakaStartRot;
            }
        }
    }

    private void DoBigHeartAnimation()
    {
        playerAnimator.SetTrigger("BigHeart");
        StartCountDown();
    }

    private void DoSmallHeartAnimation()
    {
        playerAnimator.SetTrigger("SmallHeart");
        StartCountDown();
    }

    private void DoNyanAnimation()
    {
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
