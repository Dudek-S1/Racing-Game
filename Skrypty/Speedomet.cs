using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Speedomet : MonoBehaviour
{
    public carController carController;

    public GameObject needle;
    public TextMeshProUGUI kph;
    public TextMeshProUGUI gearNumText;
    private float startPosition = -6, endPosition = -239;
    private float desiredPostion;
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        kph.text = carController.KPH.ToString("0");
        //vehicleSpeed = carController.KPH;
        UpadateNeedle();
    }
    public void UpadateNeedle()
    {
        desiredPostion = startPosition - endPosition;
        float temp = carController.engineRPM / 10000;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPostion));
    }
    public void ChangeGear()
    {
        gearNumText.text = (!carController.reverse) ? (carController.gearNum + 1).ToString() : "R";
    }
}
