using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    private float currentTime = 0f;
    private float startingTime = 3f;

    [SerializeField] TextMeshProUGUI countdownText;

    private MaidPathing _maidPathing;

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

        _maidPathing = FindObjectOfType<MaidPathing>();
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
            _maidPathing.CountDownEnd();
            Destroy(gameObject);
        }
    }
}
