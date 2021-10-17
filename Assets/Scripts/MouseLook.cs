using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;

    public Transform playerBody;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Getting player input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Stops the camera from going insane on Start
        if(Mathf.Abs(mouseY) > 100)
        {
            mouseY = 0;
        }
        if(Mathf.Abs(mouseX) > 100)
        {
            mouseX = 0;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limits the vertical look

        // Looking up/down rotates the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Looking left/right rotates the player model
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
