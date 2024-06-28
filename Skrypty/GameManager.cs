using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;
//using UnityEditor.Search;
using System.Linq;
using static RaceRanking;
using System.IO;

public class GameManager : MonoBehaviour
{
    SkyManager skyManager;
    public carController carController;

    /*public GameObject needle;
    public TextMeshProUGUI kph; 
    public TextMeshProUGUI gearNumText;
    private float startPosition =-6, endPosition=-239;
    private float desiredPostion;*/

    public float vehicleSpeed;
    // Start is called before the first frame update

    public TMP_Dropdown shifterDropdown;
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;
    private float countdownTimer;
    public bool GameStart = false;
    public GameObject settingsPanel;
    //public bool GameStart2 = false;

    public float timeElapsed = 0; // Czas, który up³yn¹³ w sekundach
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    public List<string> resultsOfRace = new List<string>();

    public GameObject endText;
    //public GameObject endText2;
    public GameObject loseText;
    //public GameObject loseText2;

    public int whichShift = 0;

    public GameObject enterNameGM;
    public TMP_InputField inputField;
    private string racerNickname;

    public GameObject rangingGM;
    public TextMeshProUGUI rankingText;

    public TMP_Dropdown driveType_Dropdown;
    public int whichDriveType = 0;

    public GameObject pauseMenu_Panel;
    private int stateOfPausePanel = 0;

    void Start()
    {
        Time.timeScale = 1;
        countdownTimer = countdownDuration;
        //StartCoroutine(StartCountdown());
        skyManager = GetComponent<SkyManager>();
        ManagingWeather();
        ManagingPartOfADay();
    }
    private void Update()
    {
        PauseGame();

        if (shifterDropdown.value == 0)
        {
            whichShift = 0;
        }else if(shifterDropdown.value == 1)
        {
            whichShift = 1;
        }


        if (driveType_Dropdown.value == 0)
        {
            whichDriveType = 0;
        }
        else if (driveType_Dropdown.value == 1) {
            whichDriveType = 1;
        }
        else if (driveType_Dropdown.value == 2)
        {
            whichDriveType = 2;
        }

        StartTimer();
        //PlayerPrefs.DeleteAll();

    }
    public void StartRace()
    {
        settingsPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    public void Win1()
    {
        //endText2.SetActive(false);
        endText.SetActive(true);

        //loseText2.SetActive(true);
        loseText.SetActive(false);

        timerIsRunning = false;
        EnableEnteringNameField();

        //StartCoroutine(LoadMenuAfterDelay()); ------
    }
    private void EnableEnteringNameField()
    {
        enterNameGM.SetActive(true);
    }
    public void GetRacerName()
    {
        racerNickname = inputField.text;
        enterNameGM.SetActive(false);
        //Debug.Log(racerNickname);
        MakeRanking();
    }

    private void MakeRanking()
    {
        rangingGM.SetActive(true);
        /*RaceRanking ranking = new RaceRanking();
        string selectedShifter = shifterDropdown.options[shifterDropdown.value].text;
        string selectedDriveType = driveType_Dropdown.options[driveType_Dropdown.value].text;
        ranking.AddPlayer(new Player(racerNickname, ConvertToFloat(timeText.text), selectedShifter, selectedDriveType));

        string json = ranking.ToJson();
        File.WriteAllText(Application.dataPath + "/saveFile.json", json);


        string filePath = Application.dataPath + "/saveFile.json";

        if (File.Exists(filePath))
        {
            string pobJson = File.ReadAllText(filePath);
            RaceRanking loadedRanking = new RaceRanking();
            loadedRanking.FromJson(pobJson);
            List<Player> sortPlayers = loadedRanking.GetPlayers();

            foreach (var player in sortPlayers)
            {
                Debug.Log($"Name: {player.Name}, RaceTime: {player.RaceTime}, Gearbox: {player.GearboxType}, Drive: {player.DriveType}");
                rankingText.text += $"Name: {player.Name}, RaceTime: {player.RaceTime}, Gearbox: {player.GearboxType}, Drive: {player.DriveType}\n";
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }*/
        /* string json = ranking.ToJson();
         Debug.Log("Serialized JSON: " + json);
         File.WriteAllText(Application.dataPath + "saveFile.json", json);

         ranking.FromJson(json);
         List<Player> sortedPlayers = ranking.GetPlayers();
         foreach (var player in sortedPlayers)
         {
             Debug.Log($"Name: {player.Name}, RaceTime: {player.RaceTime}, Gearbox: {player.GearboxType}, Drive: {player.DriveType}");
         }*/
        /*
                string pobJson = File.ReadAllText(Application.dataPath + "saveFile.json");
                RaceRanking loadedRanking = JsonUtility.FromJson<RaceRanking>(pobJson);
                List<Player> sortPlayers = loadedRanking.GetPlayers();
                foreach (var player in sortPlayers)
                {
                    Debug.Log($"Name: {player.Name}, RaceTime: {player.RaceTime}, Gearbox: {player.GearboxType}, Drive: {player.DriveType}");
                }*/

        if (PlayerPrefs.HasKey("records"))
        {
            string cos = PlayerPrefs.GetString("records");
            string formattedNickName = cos + racerNickname + " " + timeText.text + "\n";
            PlayerPrefs.SetString("records", formattedNickName);
            rankingText.text = PlayerPrefs.GetString("records");
        }
        else
        {
            string formattedNickName = racerNickname + " " + timeText.text + "\n";
            PlayerPrefs.SetString("records", formattedNickName);
            rankingText.text = PlayerPrefs.GetString("records");
        }
    }
    private string SortedRanking(string lista)
    {
        float timeInNumbers = 0;
        Dictionary<string, float> unsortedRanking = new Dictionary<string, float>();
        string[] x = lista.Split("\n");

        foreach (string s in x)
        {
            string[] a = s.Split(" ");

            timeInNumbers = ConvertToFloat(a[1]);

            unsortedRanking.Add(a[0], timeInNumbers);

        }
        var sortedDictByOrder = unsortedRanking.OrderBy(x => x.Value);

        string listOfRanking = "";
        foreach (KeyValuePair<string, float> pair in sortedDictByOrder)
        {
            listOfRanking += pair.Key + " " + ConvertToString(pair.Value) + "\n";
        }

        return listOfRanking;
    }

    public float ConvertToFloat(string timeString)
    {
        string[] timeParts = timeString.Split(':');
        if (timeParts.Length == 3)
        {
            float minutes = float.Parse(timeParts[0]);
            float seconds = float.Parse(timeParts[1]);
            float milliseconds = float.Parse(timeParts[2]);

            return (minutes * 60) + seconds + (milliseconds / 1000);
        }
        else
        {
            Debug.LogError("Invalid time format. Correct format is MM:SS:FFF");
            return 0f;
        }
    }
    public string ConvertToString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = (time % 1) * 1000;

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
    public void Win2()
    {
        endText.SetActive(false);
        //endText2.SetActive(true);

        //loseText2.SetActive(false);
        loseText.SetActive(true);

        StartCoroutine(LoadMenuAfterDelay());
    }

    /* private void FixedUpdate()
     {
         kph.text = carController.KPH.ToString("0");
         //vehicleSpeed = carController.KPH;
         UpadateNeedle();
     }
     public void UpadateNeedle()
     {
         desiredPostion = startPosition - endPosition;
         float temp = carController.engineRPM / 10000;
         needle.transform.eulerAngles = new Vector3(0, 0,(startPosition - temp * desiredPostion));
     }
     public void ChangeGear()
     {
         gearNumText.text = (!carController.reverse) ? (carController.gearNum + 1).ToString() : "R";
     }*/

    private void StartTimer()
    {
        if (timerIsRunning)
        {
            timeElapsed += Time.deltaTime;
            DisplayTime(timeElapsed);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliseconds = (timeToDisplay % 1) * 1000;

        timeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
    public void ClickBackButton()
    {
        SceneManager.LoadScene("Menu");
    }
    private void ManagingWeather()
    {
        if (StaticMenuSettingsData.valueOfWeatherType == 0) Debug.Log("pogoda 0");
        else if (StaticMenuSettingsData.valueOfWeatherType == 1) Debug.Log("pogoda 1");
        else if (StaticMenuSettingsData.valueOfWeatherType == 2) Debug.Log("pogoda 2");
    }
    private void ManagingPartOfADay()
    {
        GameObject skyManagerObject = GameObject.Find("SkyManager");
        skyManager = skyManagerObject.GetComponent<SkyManager>();

        if (StaticMenuSettingsData.valueOfPartOfADay == 0) skyManager.DaySky();
        else if (StaticMenuSettingsData.valueOfPartOfADay == 1) skyManager.NightSky();
        //skyManager.SkyMaterial(StaticMenuSettingsData.valueOfPartOfADay);

    }

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (stateOfPausePanel == 0)
            {
                Time.timeScale = 0;
                pauseMenu_Panel.SetActive(true);
                stateOfPausePanel = 1;
            }
            else if (stateOfPausePanel == 1)
            {
                Time.timeScale = 1;
                pauseMenu_Panel.SetActive(false);
                stateOfPausePanel = 0;
            }
        }
    }
    public void UnpauseGame()
    {
         Time.timeScale = 1;
         pauseMenu_Panel.SetActive(false);
         stateOfPausePanel = 0;
    }

    IEnumerator StartCountdown()
    {
        countdownText.text = "";
        while (countdownTimer > 0)
        {
            countdownText.text = Mathf.CeilToInt(countdownTimer).ToString();

            if (countdownTimer == 1f)
            {
                countdownText.color = Color.green;
            }
            else if (countdownTimer == 2f)
            {
                countdownText.color = Color.yellow;
            }
            else if (countdownTimer == 3f)
            {
                countdownText.color = Color.red;
            }

            yield return new WaitForSeconds(1f);
            countdownTimer -= 1f;
        }

        countdownText.text = "GO!";
        GameStart = true;
        timerIsRunning=true;
        //GameStart2 = true;
        countdownText.text = "";
    }

    IEnumerator LoadMenuAfterDelay()
    {
        // Poczekaj 3 sekundy
        yield return new WaitForSeconds(5f);

        // Za³aduj scenê menu
        SceneManager.LoadScene("Menu");
    }
}
