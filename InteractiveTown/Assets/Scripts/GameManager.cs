using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    
    #region Data
    public static GameManager instance; //singleton
    [SerializeField] private CameraController _activeCamera;
    public CameraController activeCamera { get { return _activeCamera; } }

    private Player _player;
    public Player player { get { return _player; } }

    [Tooltip("Layer mask for collision checking on cursor click.")]
    public LayerMask cursorLayerMask;
    public float cursorRaycastDist;

    public GameObject target;
    #endregion


    private void Awake()
    {
        instance = this;

        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        switch (Cursor.lockState) {
            case CursorLockMode.None:
                {
                    if (Input.GetMouseButton(0))
                    {
                        ProcessCursorClick();
                    }
                    break;
                }
        }   
    }

    private void ProcessCursorClick()
    {
        RaycastHit hit;
        Ray cursorRay = _activeCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(cursorRay, out hit, cursorRaycastDist, cursorLayerMask))
        {
            switch (LayerMask.LayerToName(hit.collider.gameObject.layer))
            {
                case "NPC":
                    {
                        if (!target)
                        {
                            target = hit.collider.gameObject;
                            _activeCamera.ProcessCameraTransition(target);
                        }
                        else
                            break;

                        break;
                    }
            }
        }
    }

    public void FinishedWithTarget()
    {
        target = null;
    }
}
