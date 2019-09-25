using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CameraState
{
    None, Follow, Transitioning, Finished
}

public class CameraController : MonoBehaviour
{
    #region Data
    public static CameraController instance; // Will change this from a singleton later.
    [SerializeField] private CameraState c_state; //state machine for camera state
    public GameObject target; //reference to target
    private Transform p_target; //reference to previous target
    private Vector3 t_pos; //reference to target position
    [SerializeField] private Vector3 p_pos; //reference to previous target position
    
    public float cameraHeight; //height
    public float cameraDistance; //distance from target
    public float smoothSpeed; //smooth step speed

    private Vector3 velocity; //reference to velocity per smooth step

    //camera movement based on mouse input
    private float xRot;
    private float yRot;

    //x value is min angle, y value is max value.
    public Vector2 camMinMaxAngle;
  
    public float t_rotationSpeed; //transition rotation speed
    public float t_positionSpeed; //transition position speed

    //booleans to handle completion of transition
    private bool p_transition;
    private bool r_transition;
    #endregion

    private void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        c_state = CameraState.Follow;
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        //TODO: Make a state machine for camera controller
        HandleCameraInput();
        HandleCursorState();

        t_pos = target.transform.position;

        if (c_state == CameraState.Follow)
        {
            if (target == GameManager.instance.player.lookAtObject)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    HandleCameraPosition();
                    HandleCameraRotation();
                }
            }
        }
        if(c_state == CameraState.Transitioning)
        {
            CameraTPosition();
            CameraTRotation();

            if (p_transition && r_transition)
            {
                c_state = CameraState.Follow;
                p_transition = false;
                r_transition = false;
            }
        }
        if (target != GameManager.instance.player.lookAtObject)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ProcessCameraTransition(GameManager.instance.player.lookAtObject);
            }
        }
        Debug.DrawLine(transform.position, target.transform.position, Color.red);
    }
    /// <summary>
    /// Method to handle cursor state.
    /// </summary>
    private void HandleCursorState()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    /// <summary>
    /// Camera input
    /// </summary>
    private void HandleCameraInput()
    {
        xRot += Input.GetAxis("Mouse X");
        yRot -= Input.GetAxis("Mouse Y");

        yRot = Mathf.Clamp(yRot, camMinMaxAngle.x, camMinMaxAngle.y);
    }
    /// <summary>
    /// Function calculates camera rotation while on player
    /// </summary>
    private void HandleCameraRotation()
    {
        Vector3 relativePos = t_pos - transform.position;

        Quaternion newRotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.rotation = newRotation;
    }
    /// <summary>
    /// Function calculates camera position while on player
    /// </summary>
    private void HandleCameraPosition()
    {
        Vector3 offset = new Vector3(0, 0, -cameraDistance); // offset from camera's current position.
        Quaternion rotation = Quaternion.Euler(yRot, xRot, 0);
        Vector3 cameraPosition = t_pos + rotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref velocity, smoothSpeed);
    }
    /// <summary>
    /// Function to calculate position while transitioning to new target
    /// </summary>
    private void CameraTPosition()
    {
        Vector3 offset = new Vector3(0, 0, cameraDistance);
        Vector3 cameraPosition = t_pos + offset;

        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref velocity, t_positionSpeed);

        float magnitude = (transform.position - cameraPosition).magnitude;
        if(magnitude <= 0.1f)
        {
            p_transition = true;
        }
    }
    /// <summary>
    /// Function to calculate rotation while transition to new target 
    /// </summary>
    private void CameraTRotation()
    {
        Vector3 relativePos = t_pos - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * t_rotationSpeed);
        if(Quaternion.Angle(transform.rotation, newRotation) < 1f)
        {
            r_transition = true;
        }
    }
    /// <summary>
    /// Public method to handle changing targets
    /// </summary>
    /// <param name="target"></param>
    public void ProcessCameraTransition(GameObject target)
    {
        if (target != GameManager.instance.player.lookAtObject)
        {
            p_pos = transform.position;
            t_pos = target.transform.position;
        }
        else
        {
            t_pos = p_pos;
        }
        this.target = target;
        c_state = CameraState.Transitioning;
    }
}
