using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountUpTimer : MonoBehaviour
{
    public float timeElapsed = 0; // Czas, który up³yn¹³ w sekundach
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
   // public GameObject raceTimer;
    //public Button startButton; // Przypisz przycisk startowy w Inspectorze

    private void Start()
    {
        // Przypisz funkcjê StartTimer do przycisku
        //startButton.onClick.AddListener(StartTimer);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            timeElapsed += Time.deltaTime;
            DisplayTime(timeElapsed);
        }
    }

    public void StartTimer()
    {
       // raceTimer.SetActive(true);
        timerIsRunning = true;
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
