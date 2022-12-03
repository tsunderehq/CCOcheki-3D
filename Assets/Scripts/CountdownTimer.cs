using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    private float currentTime = 0f;
    private float startingTime = 3f;

    [SerializeField] TextMeshProUGUI countdownText;

    void Start()
    {
        currentTime = startingTime;
    }

    
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime*2);

        currentTime -= 1 * Time.deltaTime;

        if (startingTime - currentTime >= 1)
        {
            transform.localScale = Vector3.one;
            startingTime = currentTime;

            countdownText.text = currentTime.ToString("0");
        }


        if (currentTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
