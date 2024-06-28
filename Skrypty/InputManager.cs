using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool handbrake;

    // Update is called once per frame
    void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        handbrake = (Input.GetAxis("Jump") !=0 )?true : false;
    }
}
