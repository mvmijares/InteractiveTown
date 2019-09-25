using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Data
    private CameraController camController;
    private int _xInput; // right/left
    private int _yInput; // jump

    private int _zInput; // forward/backward
    public Vector3 movement;
    public float moveSpeed;
    public float rotationSpeed;
    public GameObject lookAtObject;
    #endregion
    private void Start()
    {
        camController = CameraController.instance;
    }
    private void Update()
    {
        HandlePlayerInput();
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandlePlayerMovement();
            HandlePlayerRotation();
        }
    }
    private void HandlePlayerRotation()
    {
        Vector3 forward = camController.transform.forward;
        forward.y = 0; // so the player does not rotate on the x axis.
        Quaternion newRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandlePlayerMovement()
    {
        transform.position += (transform.forward * _zInput + transform.right * _xInput) * moveSpeed * Time.deltaTime;
    }

    private void HandlePlayerInput()
    {
        if (Input.GetKey(KeyCode.W))
            _zInput = 1;
        else if (Input.GetKey(KeyCode.S))
            _zInput = -1;
        else
            _zInput = 0;

        if (Input.GetKey(KeyCode.D))
            _xInput = 1;
        else if (Input.GetKey(KeyCode.A))
            _xInput = -1;
        else
            _xInput = 0;
    }
}
