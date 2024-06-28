using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class carController : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField]private driveType drive;

    internal enum gearBox
    {
        automatic,
        manual
    }
    [SerializeField] private gearBox gearChange;

    public GameManager gameManager;
    public Speedomet speedomet;

    [Header("Variables")]
    public AnimationCurve enginePower;
    public float totalPower;
    public float wheelsRPM;
    public float KPH;
    public float maxRPM, minRPM;
    public float motorTorque = 100;
    public float smoothTime = 0.01f;
    public float engineRPM;
    public float[] gears;
    public int gearNum = 0;
    public bool reverse = false;
    public float handBrakeFrictionMultiplier = 2f;

    private float radius =6,brakePower = 50000, downForceValue = 100f, driftFactor;

    private InputManager inputManager;
    private GameObject wheelMeshes, wheelColliders;
    //private GameObject rightLight, leftLight;
    public GameObject carLights;
    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];
    public GameObject centerOfMass;
    private Rigidbody rigidbody;
    public Joystick joystick;

    [Header("Debug")]
    public float[] slip = new float[4];

    private int finishLineConuter = 0;
    public GameObject endText;

    public GameObject buttonPlus;
    public GameObject buttonMinus;


    private WheelFrictionCurve forwardFriction, sidewaysFriction;

    private int lightsSwitch = 0;
    // Start is called before the first frame update
    void Awake()
    {
        GetObjects();
        StartCoroutine(timedLoop());
    }
    void Update()
    {
        ManualShifter();
        CarLights();
        if (gameManager.whichShift == 0)
        {
            gearChange = gearBox.automatic;
            //buttonPlus.SetActive(false);
            //buttonMinus.SetActive(false);
        }
        else if (gameManager.whichShift == 1) { 
            gearChange = gearBox.manual;
            //buttonPlus.SetActive(true);
            //buttonMinus.SetActive(true);
        }
    
        if(gameManager.whichDriveType==0) drive = driveType.frontWheelDrive;
        else if (gameManager.whichDriveType == 1) drive = driveType.rearWheelDrive;
        else if (gameManager.whichDriveType == 2) drive = driveType.allWheelDrive;
    }
    // Update is called once per frame
    void FixedUpdate()

    {
        AddDownForce();
        AnimateWheels();
        if (gameManager.GameStart)
        {
            MoveVehicle();
        }
            
        SteerVehicle();
        //getFriction();
        CalculatingEnginePower();
        AutomaticShifter();
        GetBackToRoad();
        adjustTraction();

    }

    private void CalculatingEnginePower()
    {
        WheelRPM();

        //totalPower = enginePower.Evaluate(engineRPM) * (gears[gearNum]) * joystick.Vertical; 
        totalPower = enginePower.Evaluate(engineRPM) * (gears[gearNum]) * inputManager.vertical;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) *3.6f * (gears[gearNum])), ref velocity, smoothTime);

    }
    private void WheelRPM()
    {
        float sum = 0;
        int R = 0;
        for (int i = 0; i< 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }
        wheelsRPM =(R!=0) ? sum/R : 0;

        if(wheelsRPM < 0 && !reverse) {
            reverse = true;
            speedomet.ChangeGear();
        }
        else if(wheelsRPM>0 && reverse){
            reverse = false;
            speedomet.ChangeGear();
        }
    }
    private void AutomaticShifter()
    {
        if (gearChange == gearBox.automatic)
        {
            if (engineRPM > maxRPM && gearNum < gears.Length - 1)
            {
                gearNum++;
                speedomet.ChangeGear();
            }
            if (engineRPM < minRPM && gearNum > 0)
            {
                gearNum--;
                speedomet.ChangeGear();
            }
        }
    }
    private void ManualShifter()
    {
        if (gearChange == gearBox.manual)
        {
            if (Input.GetKeyDown(KeyCode.E) && gearNum < gears.Length - 1)
            {
                gearNum++;
                speedomet.ChangeGear();
            }
            if (Input.GetKeyDown(KeyCode.Q) && gearNum > 0)
            {
                gearNum--;
                speedomet.ChangeGear();
            }
        }
    }
    public void IncreaseGear()
    {
        if (gearChange == gearBox.manual && gearNum < gears.Length - 1)
        {
            gearNum++;
            speedomet.ChangeGear();
        }

    }

    // Funkcja wywo³ywana po naciœniêciu przycisku "Q"
    public void DecreaseGear()
    {
        if (gearChange == gearBox.manual && gearNum > 0)
        {
            gearNum--;
            speedomet.ChangeGear();
        }
    }

    private bool IsGrounded()
    {
        if (wheels[0].isGrounded && wheels[1].isGrounded && wheels[2].isGrounded && wheels[3].isGrounded)
            return true;
        else
            return false;
    }
    private void MoveVehicle()
    {
        brakeVehicle();

        if (drive == driveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = (totalPower / 4);
            }
        }else if(drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = (totalPower/ 2);
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length-2; i++)
            {
                wheels[i].motorTorque = (totalPower / 2);
            }
        } 
        
        KPH = rigidbody.velocity.magnitude *3.6f;

        /*if (inputManager.handbrake)
        {
            wheels[2].brakeTorque = wheels[2].brakeTorque = brakePower;
        }
        else
        {
            wheels[2].brakeTorque = wheels[2].brakeTorque = 0;
        }*/

        /*for (int i = 0; i < wheels.Length - 2; i++)
        {
            wheels[i].steerAngle = 0;
        }*/
    }

    private void brakeVehicle()
    {

        if (inputManager.vertical < 0)
        {
            brakePower = (KPH >= 10) ? 500 : 0;
        }
        else if (inputManager.vertical == 0 && (KPH <= 10 || KPH >= -10))
        //else if (joystick.Vertical == 0 && (KPH <= 10 || KPH >= -10))
        {
            brakePower = 10;
        }
        else
        {
            brakePower = 0;
        }


    }
    private void SteerVehicle()
    {

        //dla telefonu zmien  "inputManager.horizontal" na "joystick.Horizontal" !!!!!!!
        if (inputManager.horizontal > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.horizontal;

        }else if(inputManager.horizontal < 0) {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.horizontal;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }
    }
    void AnimateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for(int i = 0; i<4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }
    private void GetObjects()
    {
        wheelColliders = GameObject.Find("WheelColliders");
        wheelMeshes = GameObject.Find("WheelMeshes");
        //endText = GameObject.Find("EndText");
        wheels[0] = wheelColliders.transform.Find("0").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("1").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("2").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("3").gameObject.GetComponent<WheelCollider>();


        wheelMesh[0] = wheelMeshes.transform.Find("0").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("1").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("2").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("3").gameObject;

        //carLights = GameObject.Find("Lights");


        inputManager = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
        speedomet = GetComponent<Speedomet>();
        centerOfMass = GameObject.Find("mass");
        rigidbody.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void AddDownForce()
    {
        rigidbody.AddForce(-transform.up * downForceValue * rigidbody.velocity.magnitude);
    }

    public void CarLights()
    {
        if (Input.GetKeyDown(KeyCode.L) && lightsSwitch == 0)
        {
            carLights.SetActive(true);
            lightsSwitch = 1;
        }
        else if (Input.GetKeyDown(KeyCode.L) && lightsSwitch == 1)
        {
            lightsSwitch = 0;
            carLights.SetActive(false);
        }

        /*if (lightsSwitch==0) {
            carLights.SetActive(true);
            lightsSwitch = 1;
        }else if (lightsSwitch == 1)
        {
            lightsSwitch = 0;
            carLights.SetActive(false);
        }*/
    }
    private void getFriction()
    {
        for(int i = 0;i<wheels.Length;i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);

            slip[i] = wheelHit.forwardSlip;
        }
    }

    private void GetBackToRoad()
    {
        if (transform.position.y < -20)
        {
            transform.position = new Vector3(0, 20,0);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void adjustTraction()
    {
        //tine it takes to go from normal drive to drift 
        float driftSmothFactor = .7f * Time.deltaTime;

        if (inputManager.handbrake)
        {
            sidewaysFriction = wheels[0].sidewaysFriction;
            forwardFriction = wheels[0].forwardFriction;

            float velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakeFrictionMultiplier, ref velocity, driftSmothFactor);

            for (int i = 0; i < 4; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            //extra grip for the front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }
            rigidbody.AddForce(transform.forward * (KPH / 400) * 10000);
        }
        //executed when handbrake is being held
        else
        {

            forwardFriction = wheels[0].forwardFriction;
            sidewaysFriction = wheels[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                ((KPH * handBrakeFrictionMultiplier) / 300) + 1;

            for (int i = 0; i < 4; i++)
            {
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;

            }
        }

        //checks the amount of slip to control the drift
        for (int i = 2; i < 4; i++)
        {

            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);
            //smoke


            if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -inputManager.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + inputManager.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
        }

    }

    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + KPH / 20;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        finishLineConuter++;
        if (finishLineConuter == 1)
        {
            /*Debug.Log("You finished race!!!");
            endText.SetActive(true);*/
            gameManager.resultsOfRace.Add("Player1");
            gameManager.Win1();
        }
    }
}
