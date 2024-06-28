using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameButtons : MonoBehaviour
{
    public TMP_Dropdown weatherTypeDropdown;
    public TMP_Dropdown partOfaDayDropdown;
    public TMP_Dropdown SelMapDropdown;
    public GameObject settingPanel;

    public int whichWeather = 0;
    public int whichPartOfADay = 0;
    public int whichMap = 0;

    public void Graj()
    {
        //SceneManager.LoadScene("PhoneGame");
        //SceneManager.LoadScene("WebGame");

        if (SelMapDropdown.value == 0)
        {
            SceneManager.LoadScene("WindowsGame");
        }
        else if (SelMapDropdown.value == 1)
        {
            SceneManager.LoadScene("ForestMap");
        }
    }
    public void Options_On()
    {
        settingPanel.SetActive(true);
    }
    public void Options_Off()
    {
        settingPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void WeatherTypeListener()
    {
        if (weatherTypeDropdown.value == 0)
        {
            StaticMenuSettingsData.valueOfWeatherType = 0;
        }else if (weatherTypeDropdown.value == 1)
        {
            StaticMenuSettingsData.valueOfWeatherType = 1;
        }
        else if (weatherTypeDropdown.value == 2)
        {
            StaticMenuSettingsData.valueOfWeatherType = 2;
        } 
    }

    public void PartOfADayListener()
    {
        if (partOfaDayDropdown.value == 0)
        {
            StaticMenuSettingsData.valueOfPartOfADay = 0;
        }
        else if (partOfaDayDropdown.value == 1)
        {
            StaticMenuSettingsData.valueOfPartOfADay = 1;
        }
    }
}
