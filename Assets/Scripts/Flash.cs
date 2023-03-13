using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// do the camera-like flash animation
/// </summary>
public class Flash : MonoBehaviour
{
    [SerializeField] private float flashTimelength = 0.2f;
    
    private Image flashImage;
    private float startTime;
    private bool flashing = false;

    private bool _doCameraFlash = false;

    public bool DoCameraFlash
    {
        get { return _doCameraFlash; }
        set { _doCameraFlash = value; }
    }

    void Start()
    {
        flashImage = GetComponent<Image>();
        Color col = flashImage.color;
        col.a = 0.0f;
        flashImage.color = col;
    }

    // remove later this update, but instead call CameraFlash in events
    void Update()
    {
        if (_doCameraFlash && !flashing)
        {
            CameraFlash();
        }
        else
        {
            _doCameraFlash = false;
        }
    }

    public void CameraFlash()
    {
        // initial color
        Color col = flashImage.color;

        // start time to fade over time
        startTime = Time.time;

        // so we can flash again
        _doCameraFlash = false;

        // start it as alpha = 1.0 (opaque)
        col.a = 1.0f;

        // flash image start color
        flashImage.color = col;

        // flag we are flashing so user can't do 2 of them
        flashing = true;

        StartCoroutine(FlashCoroutine());
    }
    IEnumerator FlashCoroutine()
    {
        bool done = false;

        while (!done)
        {
            float perc;
            Color col = flashImage.color;

            perc = Time.time - startTime;
            perc = perc / flashTimelength;

            if (perc > 1.0f)
            {
                perc = 1.0f;
                done = true;
            }

            col.a = Mathf.Lerp(1.0f, 0.0f, perc);
            flashImage.color = col;
            flashing = true;

            yield return null;
        }

        flashing = false;

        yield break;
    }
}