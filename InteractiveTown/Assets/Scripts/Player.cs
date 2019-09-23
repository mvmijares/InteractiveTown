using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Data
    private int _xInput; // right/left
    private int _yInput; // jump

    private int _zInput; // forward/backward
    public Vector3 movement;
    public float moveSpeed;

    #endregion

    private void Update()
    {
        HandlePlayerInput();
        HandlePlayerMovement();
        
    }

    private void HandlePlayerMovement()
    {
        movement = new Vector3(_xInput, _yInput, _zInput);
        transform.position += movement * moveSpeed * Time.deltaTime;
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
