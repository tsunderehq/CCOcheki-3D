using UnityEngine;
using TMPro;

/// <summary>
/// A singleton used to show the countdown when taking a picture
/// </summary>
public class CountdownTimer : MonoBehaviour
{
    private float currentTime = 0f;
    [SerializeField] private float startingTime = 6f; // the amount of time to count down

    [SerializeField] TextMeshProUGUI countdownText;

    private ChekiLogic _chekiLogic;

    private static CountdownTimer _instance;
    public static CountdownTimer Instance
    {
        get { return _instance; }
        set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else
            {
                Destroy(value.gameObject);
            }
        }
    }
    private void Awake()
    {
        Instance = this;

        _chekiLogic = FindObjectOfType<ChekiLogic>();
        currentTime = startingTime;
    }

    
    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;

        if (startingTime - currentTime >= 1)
        {
            transform.localScale = Vector3.one;
            startingTime = currentTime;

            countdownText.text = currentTime.ToString("0");
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 2);

        if (currentTime < 0f)
        {
            _chekiLogic.CountDownEnd();
            Destroy(gameObject);
        }
    }
}
