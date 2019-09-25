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
    private GameManager gm_instance;
    private Player p_instance;

    [SerializeField] private CameraState c_state; //state machine for camera state
    public GameObject target; //reference to target
    private GameObject pTarget;
    private Vector3 camOffset; //offset position from target
    public float cameraDistance; //distance from target
  

    //camera movement based on mouse input
    private float xRot;
    private float yRot;

    //x value is min angle, y value is max value.
    public Vector2 camMinMaxAngle;
  
    public float t_rotationSpeed; //transition rotation speed
    public float t_positionSpeed; //transition position speed
    public float smoothSpeed; //smooth step speed
    private Vector3 velocity; //reference to velocity per smooth step

    //booleans to handle completion of transition
    private bool p_transition; 
    private bool r_transition;
    //position of camera while on player.
    private Vector3 pPosition;
    #endregion
    private void Awake()
    {
        instance = this;
        InitializeCameraController();
    }
    private void Start()
    {
        gm_instance = GameManager.instance;
        p_instance = gm_instance.player;
    }
    private void InitializeCameraController()
    {
        Cursor.lockState = CursorLockMode.Locked;
        c_state = CameraState.Follow;
        camOffset = new Vector3(0, 0, -cameraDistance);
        pTarget = target;
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        //TODO: Make a state machine for camera controller
        HandleCameraInput();
        HandleCursorState();

        switch (c_state)
        {
            case CameraState.Follow:
                {
                    CameraFollowState(target);
                    break;
                }
            case CameraState.Transitioning:
                {
                    CameraTransitionState(target);
                    break;
                }
        }
        if (target != p_instance.lookAtObject)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ProcessCameraTransition(p_instance.lookAtObject);
            }
        }
        Debug.DrawLine(transform.position, target.transform.position, Color.red);
    }

    private void CameraTransitionState(GameObject target)
    {
        Vector3 currPosition;
        Quaternion currRotation;

        if (target == p_instance.lookAtObject)
        {
            currPosition = Vector3.SmoothDamp(transform.position, pPosition, ref velocity, 1);
            transform.position = currPosition;
            currRotation = CalculateCameraRotation(target.transform.position);
        }
        else
        {
            currPosition = CalculateCameraPosition(Vector3.zero, 1f, -camOffset);
            currRotation = CalculateCameraRotation(target.transform.position);
        }

        float magnitude = (currPosition - transform.position).magnitude;
        float angle = Quaternion.Angle(transform.rotation, currRotation);

        if(angle <= 0.5f)
        {
            r_transition = true;
        }
        if(magnitude <= 0.5f)
        {
            p_transition = true;
        }

        if (p_transition && r_transition)
        {
            c_state = CameraState.Follow;
            p_transition = false;
            r_transition = false;
            gm_instance.FinishedWithTarget();
        }
    }

    private void CameraFollowState(GameObject target)
    {
        if (target == p_instance.lookAtObject)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Vector3 cameraInput = new Vector3(yRot, xRot, 0);
                transform.position = CalculateCameraPosition(cameraInput, 0.125f, camOffset);
                transform.rotation = CalculateCameraRotation(target.transform.position);
            }
        }
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
    /// Function calculates camera rotation while on p_instance
    /// </summary>
    private Quaternion CalculateCameraRotation(Vector3 targetPos)
    {
        Vector3 relativePos = targetPos - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * t_rotationSpeed);

        return newRotation;
    }
    /// <summary>
    /// Function calculates camera position
    /// </summary>
    private Vector3 CalculateCameraPosition(Vector3 rotation, float smoothing, Vector3 offset = default(Vector3))
    {
        Quaternion camRotation = Quaternion.Euler(rotation);
        Vector3 cameraPosition = target.transform.position + camRotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref velocity, smoothing);

        return cameraPosition;
    }
    /// <summary>
    /// Public method to handle changing targets
    /// </summary>
    /// <param name="target"></param>
    public void ProcessCameraTransition(GameObject target)
    {
        if (target != p_instance.lookAtObject)
            pPosition = transform.position;

        pTarget = this.target;
        this.target = target;
        c_state = CameraState.Transitioning;
    }
}
