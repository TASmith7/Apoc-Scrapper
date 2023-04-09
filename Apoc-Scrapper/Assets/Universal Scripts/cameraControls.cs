using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [SerializeField] int sensHorizontal;
    [SerializeField] int sensVertical;

    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invertY;

    float xRotation;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        // Get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVertical;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHorizontal;

        // Convert input to rotation float 
        if (invertY)
        {
            xRotation += mouseY;
        }
        else
        {
            xRotation -= mouseY;
        }

       

        // Clamp camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        // Rotate the camera on the x axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotate the player on the y axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
