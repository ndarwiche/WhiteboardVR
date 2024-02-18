using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace WhiteboardXR.Board
{
    /// <summary>
    /// Place the board on a wall (arplane)
    /// </summary>
    public class BoardPlacer : MonoBehaviour
    {
        [SerializeField] private ARPlaneManager _arPlaneManager;
        [SerializeField] private Transform _mainCamera;
        [SerializeField] private InputActionReference _updatePositionInputAction;
        [SerializeField] private InputActionReference _updatePositionInputAction1;

        private void Start()
        {
            UpdateBoardPlacement();
            _updatePositionInputAction.action.performed += UpdatePositionActionPerformed;
            _updatePositionInputAction1.action.performed += UpdatePositionActionPerformed;
        }

        private void UpdatePositionActionPerformed(InputAction.CallbackContext action)
        {
            if (!action.performed)
                return;
            UpdateBoardPlacement();
        }

        private void UpdateBoardPlacement()
        {
            foreach (var arPlane in _arPlaneManager.trackables)
            {
                if (arPlane.classification == PlaneClassification.Wall)
                {
                    Debug.Log("Wall");
                    //Angle is smaller than ~25 degrees
                    if (Vector3.Dot(arPlane.normal, _mainCamera.forward) < -0.9f)
                    {
                        Debug.Log("Good Wall");
                        transform.position = arPlane.center;
                        transform.forward = -arPlane.normal;
                        //place it 1 cm in front of the wall
                        transform.position += transform.forward * 0.01f;
                    }
                }
            }
        }
    }
}