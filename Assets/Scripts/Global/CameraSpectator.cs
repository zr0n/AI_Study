using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpectator : MonoBehaviour
{
    public float rotationSpeed = 10f;
    private Vector3 inputAxis;
    

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime * inputAxis.y);
        transform.Rotate(transform.forward * rotationSpeed * Time.deltaTime * inputAxis.z);
        transform.Rotate(transform.right * rotationSpeed * Time.deltaTime * inputAxis.x);
    }



    void HandleKeyboardInput()
    {
        inputAxis = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
            inputAxis.y += -1;
        if (Input.GetKey(KeyCode.D))
            inputAxis.y += 1;
        if (Input.GetKey(KeyCode.E))
            inputAxis.x = 1;
        if (Input.GetKey(KeyCode.Q))
            inputAxis.x = -1;
        if (Input.GetKey(KeyCode.W))
            inputAxis.z = 1;
        if (Input.GetKey(KeyCode.S))
            inputAxis.z = -1;
    }

}
