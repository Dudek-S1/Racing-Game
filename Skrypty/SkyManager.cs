using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public float skySpeed;
    public  Material skyDay;
    public Material skyNight;

    public GameObject sceneLight;
    private Light lightComponent;

    private void Awake()
    {
        lightComponent = sceneLight.GetComponent<Light>();
    }
    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skySpeed);
    }

    /*public void SkyMaterial(int i)
    {
        if (i == 0) RenderSettings.skybox = skyDay;
        else if (i == 1) RenderSettings.skybox = skyNight;
    }*/
    public void DaySky()
    {
        //RenderSettings.skybox = skyDay;
        Debug.Log("halo");
    }
    public void NightSky()
    {
        RenderSettings.skybox = skyNight;
        lightComponent.intensity = (float)0.11;
        Debug.Log("halo noc");
    }



}
