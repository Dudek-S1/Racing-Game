using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{

    public GameObject player;
    private carController carController;
    public GameObject child;
    public float speed;
    public float defaltPOV = 0,desiredPOV=0;
    [Range(0,5)]public float smothTime = 0;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        child = player.transform.Find("CameraPosition").gameObject;
        carController = player.GetComponent<carController>();
        defaltPOV = Camera.main.fieldOfView;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Follow();
        BoostPOV();

        //speed = (carController.KPH >= 50) ? 20 : carController.KPH / 4;
    }
    private void Follow()
    {
        if (speed <= 23) {
            speed = Mathf.Lerp(speed, carController.KPH / 2, Time.deltaTime);
        }
        else { 
            speed=23;
        }

        Vector3 targetPosition = child.transform.position + new Vector3(0, 1, 0);
        //gameObject.transform.position = Vector3.Lerp(transform.position,child.transform.position + new Vector3(0,2,-4),Time.deltaTime * speed);
        gameObject.transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);

        Vector3 lookAtPosition = player.gameObject.transform.position;
        transform.LookAt(lookAtPosition);
        transform.rotation = Quaternion.Euler(10, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private void BoostPOV()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, desiredPOV, Time.deltaTime * smothTime);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaltPOV, Time.deltaTime * smothTime);
        }
    }
}
