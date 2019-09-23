using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    public float cameraHeight;
    public float cameraDistance;

    public float smoothSpeed;

    private Vector3 velocity;
    
    private void LateUpdate()
    {
        if (!target)
            return;

        HandleCameraInput();
        HandleCameraPosition();
        HandleCameraRotation();
    }

    private void HandleCameraInput()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

    }

    private void HandleCameraRotation()
    {

    }

    private void HandleCameraPosition()
    {

        Vector3 offset = target.transform.forward * -cameraDistance;
        Vector3 cameraPosition = target.transform.position + offset;
        cameraPosition.y = cameraHeight;

        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref velocity, smoothSpeed);

        Debug.Log("Updating Camera Position.");
    }
}
