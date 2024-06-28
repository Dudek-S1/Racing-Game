using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;
    private float countdownTimer;
    public bool GameStart = false;

    void Start()
    {
        countdownTimer = countdownDuration;
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownTimer > 0)
        {
            countdownText.text = Mathf.CeilToInt(countdownTimer).ToString();
            yield return new WaitForSeconds(1f);
            countdownTimer -= 1f;
        }

        countdownText.text = "GO!";
        GameStart = true;
    }
}

